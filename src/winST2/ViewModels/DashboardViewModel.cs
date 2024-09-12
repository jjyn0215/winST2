// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Collections.ObjectModel;
using Wpf.Ui.Controls;
using Wpf.Ui.Demo.Mvvm.Comms;
using Wpf.Ui.Demo.Mvvm.Helpers;
using Wpf.Ui.Demo.Mvvm.Models;
using Wpf.Ui.Demo.Mvvm.Services;

namespace Wpf.Ui.Demo.Mvvm.ViewModels;

public partial class DashboardViewModel(IDeviceUpdateService deviceUpdateService) : ViewModel
{
    [ObservableProperty]
    public bool _isInitialized = false;

    [ObservableProperty]
    private string _isVisible = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Location> _locations = [];

    [ObservableProperty]
    private string _status;

    public override void OnNavigatedTo()
    {
        if (!IsInitialized)
        {
            InitializeViewModel();
        }
    }

    public override void OnNavigatedFrom()
    {
        deviceUpdateService.StopUpdating();
    }

    [RelayCommand]
    private static async Task OnDeviceChanged(Device sender)
    {
        GetDevicesFromCloud.OpenSnackBar(sender.Status,sender.Name, ControlAppearance.Info);
        //await GetDevicesFromCloud.SendCommandAsync(sender.Key, sender.Status == "True" ? "on" : "off");
        sender.Status = await GetDevicesFromCloud.SendCommandAsync(sender.Key, sender.Status == "True" ? "on" : "off") == false ? sender.Status == "True" ? "False" : "True" : sender.Status;
        //GetDevicesFromCloud.OpenSnackBar(await GetDevicesFromCloud.SendCommandAsync(sender.Key, sender.Status == "True" ? "on" : "off") == false ? sender.Status == "True" ? "False" : "True" : null, "?", ControlAppearance.Info);
    }

    [RelayCommand]
    private static async Task OnSyncPressed(object sender)
    {
        await GetDevicesFromCloud.RequestData();
    }

    public async void InitializeViewModel()
    {
        if (await GetDevicesFromCloud.RequestData() == true)
        {
            IsInitialized = true;
            deviceUpdateService.StartUpdating();
        }
        else
        {
            IsInitialized = false;
            deviceUpdateService.StopUpdating();
        }
        
    }


}
