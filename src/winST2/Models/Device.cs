
namespace Wpf.Ui.Demo.Mvvm.Models;

public partial class Device : ObservableObject
{
    public string Name { get; set; }

    public string RoomName {  get; set; }

    public string LocationName { get; set; }

    [ObservableProperty]
    public string _status;

    public string IsOnline { get; set; }

    public string IsSwitchCapable { get; set; }

    public string Type { get; set; }

    public string Key { get; set; }
}