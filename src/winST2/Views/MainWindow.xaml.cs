// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Wpf.Ui.Abstractions;
using Wpf.Ui.Controls;
using Wpf.Ui.Demo.Mvvm.ViewModels;
using Wpf.Ui.Tray.Controls;

namespace Wpf.Ui.Demo.Mvvm.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INavigationWindow
{
    public ViewModels.MainWindowViewModel ViewModel { get; }

    public MainWindow(ViewModels.MainWindowViewModel viewModel, INavigationService navigationService, ISnackbarService snackbarService)
    {
        ViewModel = viewModel;
        DataContext = this;

        Appearance.SystemThemeWatcher.Watch(this);

        InitializeComponent();

        
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        navigationService.SetNavigationControl(RootNavigation);
       
    }


    
    public INavigationView GetNavigation() => RootNavigation;

    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    public void SetPageService(INavigationViewPageProvider navigationViewPageProvider) =>
        RootNavigation.SetPageProviderService(navigationViewPageProvider);

    public void ShowWindow() => Show();

    public void CloseWindow() => Close();

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }
   
    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            Hide(); 
        }
        base.OnStateChanged(e);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true; 
        Hide(); 
    }

    INavigationView INavigationWindow.GetNavigation()
    {
        throw new NotImplementedException();
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    private void NotifyIcon_LeftDoubleClick(NotifyIcon sender, RoutedEventArgs e)
    {
        ShowWindow();
        WindowState = WindowState.Maximized;
    }

    private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
    {
        ShowWindow();
        WindowState = WindowState.Maximized;
    }

    private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
    {
        OnClosed(e);
    }
}
