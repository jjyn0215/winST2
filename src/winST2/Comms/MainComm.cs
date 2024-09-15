using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Wpf.Ui.Controls;
using Wpf.Ui.Demo.Mvvm.Helpers;
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

    internal static void OpenSnackBar(string title, string message, ControlAppearance controlAppearance, SymbolIcon symbolIcon, int time)
    {
        var snackbarService = (ISnackbarService)App.Services.GetService(typeof(ISnackbarService));

        snackbarService?.Show(
            title,
            message,
            controlAppearance,
            symbolIcon,
            TimeSpan.FromSeconds(time)
        );
    }

    internal static async Task<bool?> RequestData()
    {
        try 
        {
            OpenSnackBar("Now loading...", "If this take too long, check your internet connection.", ControlAppearance.Info, new SymbolIcon(SymbolRegular.Info24), 30);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settingsViewModel.ApiToken);
            dashboardViewModel.Devices.Clear();
            var locations = await FetchDataAsync("/locations");
            if (locations["items"] == null || !locations["items"].Any())
            {
                return false;
            }
            var devices = await FetchDataAsync($"/devices");

            foreach (var location in locations["items"])
            {
                string locationId = location["locationId"].ToString();
                var rooms = await FetchDataAsync($"/locations/{locationId}/rooms");

                await CreateDevices(rooms, devices, location);
            }
            OpenSnackBar("Loaded Successfully.", "Devices are ready!", ControlAppearance.Success, new SymbolIcon(SymbolRegular.CheckmarkCircle24), 3);
            return true;
        }
        catch (Exception ex)
        {
            OpenSnackBar("Something went wrong.", ex.Message, ControlAppearance.Danger, new SymbolIcon(SymbolRegular.Warning24), 5);
            return false;
        }
    }

    private static async Task CreateDevices(JObject rooms, JObject devices, JToken location)
    {
        //var locationModel = new Location
        //{
        //    Name = location["name"].ToString(),
        //};
        if (rooms["items"] == null)
        {
            return;
        }
        foreach (var room in rooms["items"])
        {
            var devicesInRoom = devices["items"].Where(d => d["roomId"]?.ToString() == room["roomId"]?.ToString());
            foreach (var device in devicesInRoom)
            {
                {
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        string status = await GetStatusAsync(device["deviceId"]?.ToString());
                        dashboardViewModel.Devices.Add(new Device
                        {
                            Name = device["label"]?.ToString() ?? "Unnamed",
                            RoomName = room["name"]?.ToString(),
                            LocationName = location["name"]?.ToString(),
                            Type = WhatIsYourDevice.IconSelector(device["name"].ToString()),
                            Property = status.Split('!')[1] ?? "Unknown",
                            Status = status.Split('!')[0] == "on" ? "True" : "False",
                            IsOnline = await GetOnlineStatusAsync(device["deviceId"]?.ToString()) == "ONLINE" ? "True" : "False",
                            IsSwitchCapable = status.Split('!')[0] == "on" || status.Split('!')[0] == "off" ? "Visible" : "Hidden",
                            Key = device["deviceId"]?.ToString(),
                        });
                    });
                }
                //await CreateDevices(device, room);
            }
        }
        //dashboardViewModel.Locations.Add(locationModel);
    }

    internal static async Task UpdateDevices()
    {
        foreach (var device in dashboardViewModel.Devices)
        {
            var updateStatus = await GetStatusAsync(device.Key);
            device.Status = updateStatus.Split('!')[0] == "on" ? "True" : "False";
            device.Property = updateStatus.Split('!')[1] ?? "Unknown";
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
            OpenSnackBar("Something went wrong.", ex.Message, ControlAppearance.Danger, new SymbolIcon(SymbolRegular.Warning24), 5);
            return new JObject();
        }
    }


    internal static async Task<string> GetStatusAsync(string deviceId)
    {
        var statusResponse = await FetchDataAsync($"/devices/{deviceId}/status");
        var switchStatus = statusResponse["components"]?["main"]?["switch"]?["switch"]?["value"]?.ToString() ?? null;
        var propertyString = string.Empty;
        foreach (var property in statusResponse["components"]?["main"]?.Children<JProperty>())
        {
            foreach (var value in property.Value)
            {
                propertyString += value?.First?["value"]?.ToString() + value?.First?["unit"]?.ToString() + " ";
            }
        }
        return switchStatus + "!" + propertyString.TrimStart() ?? "Unknown";
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
            OpenSnackBar("Something went wrong.", ex.Message, ControlAppearance.Danger, new SymbolIcon(SymbolRegular.Warning24), 5);
            return false;
        }
    }
}
