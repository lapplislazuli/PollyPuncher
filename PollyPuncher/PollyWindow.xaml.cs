using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

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
            this.PollyProps = App.PollyProperties;
            this.AudioProps = App.AudioDeviceProperties;

            App.LoadSettings();
            
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
            if (string.IsNullOrEmpty(PollyProps.ApiKey) || PollyProps.ApiKey.Contains(","))
            {
                MessageBox.Show("You need to set a Key before you can use the Application","Key Required!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else
            {
                pc.Call();
            }
        }
        
        private void PlayHotKey_Executed(object sender, ExecutedRoutedEventArgs e) { PlaySound(); }

        
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PollyProps.ApiKey) || PollyProps.ApiKey.Contains(","))
            {
                MessageBox.Show("You need to set a Key before you can use the Application","Key Required!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
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
                    PollyProps.ApiKey = file;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                {
                    MessageBox.Show("You need to set a Key before you can use the Application","Key Required!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    break; 
                }
                    
            }
        }
        
        private void PollyWindow_OnClosing(object sender, CancelEventArgs e)
        {
            App.SaveSettings();
        }
    }
}