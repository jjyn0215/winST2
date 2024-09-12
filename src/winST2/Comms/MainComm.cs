﻿using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Wpf.Ui.Controls;
using Wpf.Ui.Demo.Mvvm.Models;
using Wpf.Ui.Demo.Mvvm.ViewModels;


namespace Wpf.Ui.Demo.Mvvm.Comms;
internal partial class GetDevicesFromCloud : ObservableObject
 {

    private static SettingsViewModel settingsViewModel = (SettingsViewModel)App.Services.GetService(typeof(SettingsViewModel));
    private static DashboardViewModel dashboardViewModel = (DashboardViewModel)App.Services.GetService(typeof(DashboardViewModel));


    internal static readonly HttpClient client = new HttpClient
    {
        BaseAddress = new Uri("https://api.smartthings.com/v1"),
    };

    internal static void OpenSnackBar(string title, string message, ControlAppearance controlAppearance)
    {
        var snackbarService = (ISnackbarService)App.Services.GetService(typeof(ISnackbarService));

        snackbarService?.Show(
            title,
            message,
            controlAppearance,
            new SymbolIcon(SymbolRegular.ErrorCircle24),
            TimeSpan.FromSeconds(3)
        );
    }

    internal static async Task<bool?> RequestData()
    {
        try 
        {
            OpenSnackBar("Now loading...", "If this take too long, check your internet connection.", ControlAppearance.Info);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settingsViewModel.ApiToken);
            //dashboardViewModel.Devices.Clear();
            var locations = await FetchDataAsync("/locations");
            if (locations["items"] == null || !locations["items"].Any())
            {
                return false;
            }
            foreach (var location in locations["items"])
            {
                string locationId = location["locationId"].ToString();
                var rooms = await FetchDataAsync($"/locations/{locationId}/rooms");
                var devices = await FetchDataAsync("/devices");

                await RequestDevices(rooms, devices);
            }
            OpenSnackBar("Loaded Successfully.", "Devices are ready!", ControlAppearance.Success);
            return true;
        }
        catch (Exception ex)
        {
            OpenSnackBar("Something went wrong.", ex.Message, ControlAppearance.Danger);
            return false;
        }
    }

    private static async Task RequestDevices(JObject rooms, JObject devices)
    {
        if (rooms["items"] == null)
        {
            return;
        }
        foreach (var room in rooms["items"])
        {
            var devicesInRoom = devices["items"].Where(d => d["roomId"]?.ToString() == room["roomId"]?.ToString());
            foreach (var device in devicesInRoom)
            {
                await CreateDevices(device, room);
            }
        }

    }

    private static async Task CreateDevices(JToken device, JToken room)
    {
        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            string status = await GetSwitchStatusAsync(device["deviceId"]?.ToString());
            Location._devices
            Location.Devices.Add(new Device
            {
                Name = device["label"]?.ToString() ?? "Unnamed",
                RoomName = room["name"]?.ToString(),
                Type = device["name"]?.ToString() ?? "Unknown",
                Status = status == "on" ? "True" : "False",
                IsOnline = await GetOnlineStatusAsync(device["deviceId"]?.ToString()) == "ONLINE" ? "True" : "False",
                IsSwitchCapable = status == "on" || status == "off" ? "Visible" : "Collapsed",
                Key = device["deviceId"]?.ToString(),
            });
            
        });
    }

    internal static async Task UpdateDevices()
    {
        foreach (var device in Location.Devices)
        {
            device.Status = await GetSwitchStatusAsync(device.Key) == "on" ? "True" : "False";
        }
    }

    private static async Task<JObject> FetchDataAsync(string endpoint)
    {
        try
        {
            var response = JObject.Parse(await client.GetStringAsync(endpoint));
            return response;
        }
        catch (Exception ex)
        {
            OpenSnackBar("Something went wrong.", ex.Message, ControlAppearance.Danger);
            return new JObject();
        }
    }


    internal static async Task<string> GetSwitchStatusAsync(string deviceId)
    {
        var statusResponse = await FetchDataAsync($"/devices/{deviceId}/status");
        var switchStatus = statusResponse["components"]?["main"]?["switch"]?["switch"]?["value"]?.ToString();
        return switchStatus ?? "Unknown";
    }

    internal static async Task<string> GetOnlineStatusAsync(string deviceId)
    {
        var healthResponse = await FetchDataAsync($"/devices/{deviceId}/health");
        var onlineStatus = healthResponse["state"]?.ToString();
        return onlineStatus ?? "Unknown";
    }

    internal static async Task<bool> SendCommandAsync(string deviceId, string command)
    {
        try
        {
            var commandBody = new
            {
                commands = new[]
                {
                new
                {
                    component = "main",
                    capability = "switch",
                    command = command,
                }
            }
            };

            var json = JObject.FromObject(commandBody).ToString();
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/devices/{deviceId}/commands", content);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            OpenSnackBar("Something went wrong.", ex.Message, ControlAppearance.Danger);
            return false;
        }
    }
}
