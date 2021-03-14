using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace PollyPuncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly AudioDeviceProperties AudioDeviceProperties = new AudioDeviceProperties();
        public static readonly PollyProperties PollyProperties = new PollyProperties();

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
                AudioDeviceProperties.DeviceA = int.Parse(devA);
                AudioDeviceProperties.VolumeA = double.Parse(volA,CultureInfo.InvariantCulture);
                AudioDeviceProperties.DeviceB = int.Parse(devB);
                AudioDeviceProperties.VolumeB = double.Parse(volB,CultureInfo.InvariantCulture);
            } catch (Exception e)
            {
                // Do nothing, if any issue was here just take default values
            }
            
            if (settings["KeyFile"] != null && settings["KeyFile"].Value != null)
            {
                string lastKey = settings["KeyFile"].Value;
                lastKey = lastKey.Split(",").Last();
                PollyProperties.ApiKey = lastKey;
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
            
            settings.Add("DeviceA",AudioDeviceProperties.DeviceA.ToString());
            settings.Add("VolumeA",AudioDeviceProperties.VolumeA.ToString("G",CultureInfo.InvariantCulture));
            
            settings.Add("DeviceB",AudioDeviceProperties.DeviceB.ToString());
            settings.Add("VolumeB",AudioDeviceProperties.VolumeB.ToString("G",CultureInfo.InvariantCulture));
            
            //TODO: Something is not 100% right here - the API Key gets added multiple times separated by a comma
            settings.Add("KeyFile",PollyProperties.ApiKey);
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }

}