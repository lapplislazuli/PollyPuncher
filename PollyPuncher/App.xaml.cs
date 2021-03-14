using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
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
        public static readonly AudioDeviceProperties _audioDeviceProperties = new AudioDeviceProperties();
        public static readonly PollyProperties _pollyProperties = new PollyProperties();

        /**
         * This method tries to load user-level settings specified for this App.
         * On First Startup, there will be none and default values are used.
         * This can also happen when your Roaming Data gets cleaned (Win Update).
         *
         * They are set the first time the app closes.
         */
        public static void LoadSettings()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            var settings = configFile.AppSettings.Settings;

            try
            {
                string devA = settings["DeviceA"].Value.Split(",").Last();
                string devB = settings["DeviceB"].Value.Split(",").Last();
                string volA = settings["VolumeA"].Value.Split(",").Last();
                string volB = settings["VolumeB"].Value.Split(",").Last();
                _audioDeviceProperties.deviceA = int.Parse(devA);
                _audioDeviceProperties.volumeA = double.Parse(volA,CultureInfo.InvariantCulture);
                _audioDeviceProperties.deviceB = int.Parse(devB);
                _audioDeviceProperties.volumeB = double.Parse(volB,CultureInfo.InvariantCulture);
            } catch (Exception e)
            {
                // Do nothing, if any issue was here just take default values
            }

            if (settings["KeyFile"] != null)
            {
                string lastKey = settings["KeyFile"].Value;
                if (settings["KeyFile"].Value != null)
                {
                    lastKey = lastKey.Split(",").Last();
                    _pollyProperties.apiKey = lastKey;
                }
            }
        }
        
        /**
         * This Method is run on system close and sets all settings to user-level.
         * Due to some issues with storing the variables, they are purged before newly set.
         */
        public static void SaveSettings()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            settings.Remove("DeviceA");
            settings.Remove("DeviceB");
            settings.Remove("VolumeA");
            settings.Remove("VolumeB");
            settings.Remove("KeyFile");
            
            settings.Add("DeviceA",_audioDeviceProperties.deviceA.ToString());
            settings.Add("VolumeA",_audioDeviceProperties.volumeA.ToString("G",CultureInfo.InvariantCulture));
            
            settings.Add("DeviceB",_audioDeviceProperties.deviceB.ToString());
            settings.Add("VolumeB",_audioDeviceProperties.volumeB.ToString("G",CultureInfo.InvariantCulture));
            
            //TODO: Something is not 100% right here - the API Key gets added multiple times separated by a comma
            settings.Add("KeyFile",_pollyProperties.apiKey);
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }

}