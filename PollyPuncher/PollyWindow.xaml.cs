using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        public PollyProperties PollyProps { get; set; }
        public AudioDeviceProperties AudioProps  { get; set; }

        private PollyCaller pc;
        
        
        public PollyWindow()
        {
            this.PollyProps = App._pollyProperties;
            this.AudioProps = App._audioDeviceProperties;

            App.loadSettings();
            
            pc = new PollyCaller(PollyProps, AudioProps);
            
            InitializeComponent();
            
            this.DataContext = this;
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            PlaySound();
        }

        private void PlaySound()
        {
            pc.Call();
        }
        
        private void PlayHotKey_Executed(object sender, ExecutedRoutedEventArgs e) { PlaySound(); }

        
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".mp3";
            var result = saveFileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var file = saveFileDialog.FileName;
                    pc.SaveToFile(file);
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    throw new InvalidDataException("This is not a file bro");
                    break;
            }
        }
        
        private void BtnKeyFileOpen_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.RestoreDirectory = false;
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var file = fileDialog.FileName;
                    PollyProps.apiKey = file;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    throw new InvalidDataException("This is not a file bro");
                    break;
            }
        }
        
        private void PollyWindow_OnClosing(object sender, CancelEventArgs e)
        {
            App.saveSettings();
        }
    }
}