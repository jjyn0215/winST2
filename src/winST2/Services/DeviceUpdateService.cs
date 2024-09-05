using Wpf.Ui.Demo.Mvvm.Comms;
 
namespace Wpf.Ui.Demo.Mvvm.Services
{
    public interface IDeviceUpdateService
    {
        void StartUpdating();
        void StopUpdating();
    }
    public class DeviceUpdateService : IDeviceUpdateService
    {
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(5); // 30초마다 업데이트
        private Timer _timerShort;
        private Timer _timerLong;

        public void StartUpdating()
        {
            _timerShort = new Timer(async _ => {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await GetDevicesFromCloud.UpdateDevices();
                });
            }, null, TimeSpan.FromSeconds(5), _updateInterval);

            //_timerLong = new 
        }

        public void StopUpdating()
        {
            _timerShort?.Dispose();
        }
    }
}
