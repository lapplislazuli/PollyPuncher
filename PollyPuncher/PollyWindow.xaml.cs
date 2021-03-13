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
        public PollyProperties pollyProps { get; set; }
        public AudioDeviceProperties audioProps  { get; set; }

        private PollyCaller pc;
        public PollyWindow()
        {
            this.pollyProps = App._pollyProperties;
            this.audioProps = App._audioDeviceProperties;

            pc = new PollyCaller(pollyProps, audioProps);
            
            InitializeComponent();
            
            this.DataContext = this;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            pc.Call(pollyProps.textToPlay);
        }
    }
}