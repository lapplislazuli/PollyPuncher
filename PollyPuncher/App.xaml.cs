using System;
using System.Collections.Generic;
using System.Configuration;
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

    }
}