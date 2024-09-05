using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Wpf.Ui.Demo.Mvvm.Comms;

namespace Wpf.Ui.Demo.Mvvm.Helpers;

internal class NeedToKeepYourKey
{
    private static readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Key.json");

    public static string GetApiKey()
    {
        try
        {
            CreateDefaultSettingsFile();
            var json = File.ReadAllText(_filePath);
            var jsonObj = JObject.Parse(json);
            return jsonObj["ApiKey"]?.ToString();
        }
        catch (Exception ex)
        {
            GetDevicesFromCloud.OpenSnackBar("SHIT!!!!!!!!", ex.Message, Controls.ControlAppearance.Danger);
            return null;
        }
    }

    // API 키 쓰기
    public static void UpdateApiKey(string newApiKey)
    {
        try
        {
            CreateDefaultSettingsFile();
            var json = File.ReadAllText(_filePath);
            var jsonObj = JObject.Parse(json);
            jsonObj["ApiKey"] = newApiKey;
            File.WriteAllText(_filePath, jsonObj.ToString());
        }
        catch (Exception ex)
        {
            GetDevicesFromCloud.OpenSnackBar("SHIT!!!!!!!!", ex.Message, Controls.ControlAppearance.Danger);
        }
    }

    private static void CreateDefaultSettingsFile()
    {
        if (!File.Exists(_filePath))
        {
            var defaultSettings = new JObject
            {
                { "ApiKey", "" }
            };

            try
            {
                File.WriteAllText(_filePath, defaultSettings.ToString());
            }
            catch (Exception ex)
            {
                GetDevicesFromCloud.OpenSnackBar("SHIT!!!!!!!!", ex.Message, Controls.ControlAppearance.Danger);
            }
        }
    }
}

