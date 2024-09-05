// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Wpf.Ui.Demo.Mvvm.Helpers;

namespace Wpf.Ui.Demo.Mvvm.ViewModels;

public partial class SettingsViewModel : ViewModel
{
    private static DashboardViewModel dashboardViewModel = (DashboardViewModel)App.Services.GetService(typeof(DashboardViewModel));

    private bool _isInitialized = false;

    [ObservableProperty]
    private string _appVersion = string.Empty;

    [ObservableProperty]
    private Wpf.Ui.Appearance.ApplicationTheme _currentApplicationTheme = Wpf.Ui
        .Appearance
        .ApplicationTheme
        .Unknown;

    [ObservableProperty]
    private string _apiToken = NeedToKeepYourKey.GetApiKey();

    [ObservableProperty]
    private string _checkMessage = string.Empty;

    [ObservableProperty]
    private string _fontColor = "red";

    partial void OnApiTokenChanged(string value)
    {
        if (ApiToken.Length == 36 && ApiToken.Count(c => c == '-') == 4)
        {
            CheckMessage = "Vaild key";
            FontColor = "green";
            NeedToKeepYourKey.UpdateApiKey(ApiToken);
        }
        else if (ApiToken.Length != 0)
        {
            CheckMessage = "Not a vaild key";
            FontColor = "red";
        }
        else
        {
            CheckMessage = "Key is empty.";
            FontColor = "red"; 
        }
        dashboardViewModel.IsInitialized = false;
    }

    public override void OnNavigatedTo()
    {
        if (!_isInitialized)
        {
            InitializeViewModel();
        }
        CheckMessage = ApiToken.Length == 0 ? "Key is empty." : string.Empty;
    } 

    private void InitializeViewModel()
    {
        CurrentApplicationTheme = Wpf.Ui.Appearance.ApplicationThemeManager.GetAppTheme();
        AppVersion = $"WinST2 - {GetAssemblyVersion()} with WPF UI 4.0.0-rc2";

        _isInitialized = true;
    }

    private static string GetAssemblyVersion()
    {
        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
            ?? string.Empty;
    }

    
    [RelayCommand]
    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (CurrentApplicationTheme == Wpf.Ui.Appearance.ApplicationTheme.Light)
                {
                    break;
                }

                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Light);
                CurrentApplicationTheme = Wpf.Ui.Appearance.ApplicationTheme.Light;

                break;

            default:
                if (CurrentApplicationTheme == Wpf.Ui.Appearance.ApplicationTheme.Dark)
                {
                    break;
                }

                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);
                CurrentApplicationTheme = Wpf.Ui.Appearance.ApplicationTheme.Dark;

                break;
        }
    }
}
