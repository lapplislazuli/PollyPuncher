using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PollyPuncher
{
    /// <summary>
    /// Interaction logic for PollyWindow.xaml
    /// </summary>
    public partial class PollyWindow : Window
    {
        private PollyProperties pollyProps;
        private AudioDeviceProperties audioProps;
        
        public PollyWindow()
        {

            this.pollyProps = App._pollyProperties;
            this.audioProps = App._audioDeviceProperties;

            PollyCaller pc = new PollyCaller(pollyProps, audioProps);
            
            //pc.Call("Hi");
            InitializeComponent();

        }
    }
}