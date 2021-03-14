using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PollyPuncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AudioDeviceProperties _audioDeviceProperties = new AudioDeviceProperties();
        public static PollyProperties _pollyProperties = new PollyProperties();

        public static void loadSettings()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            var settings = configFile.AppSettings.Settings;

            try
            {
                _audioDeviceProperties.deviceA = int.Parse(settings["DeviceA"].Value);
                _audioDeviceProperties.volumeA = double.Parse(settings["VolumeA"].Value);
                _audioDeviceProperties.deviceB = int.Parse(settings["DeviceB"].Value);
                _audioDeviceProperties.volumeB = double.Parse(settings["VolumeB"].Value);
            } catch (Exception e)
            {
                // Do nothing, if any issue was here just take default values
            }

            if (settings["KeyFile"] != null)
            {
                _pollyProperties.apiKey = settings["KeyFile"].Value;
            }
        }
        
        public static void saveSettings()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            var settings = configFile.AppSettings.Settings;

            settings.Add("DeviceA",_audioDeviceProperties.deviceA.ToString());
            settings.Add("VolumeA",_audioDeviceProperties.volumeA.ToString());
            
            settings.Add("DeviceB",_audioDeviceProperties.deviceB.ToString());
            settings.Add("VolumeB",_audioDeviceProperties.volumeB.ToString());

            settings.Add("KeyFile",_pollyProperties.apiKey);
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }

}