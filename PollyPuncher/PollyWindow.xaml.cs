using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
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
        // Despite IDE suggestions, these must be public, otherwise their attributes are invisible in UI.
        // Also, the get & set are needed
        public PollyProperties PollyProps
        {
            get;
            set;
        }
        public AudioDeviceProperties AudioProps { get; set; }

        private PollyPropertiesMemento PollyHistory;
        

        private PollyCaller pc;
        
        
        public PollyWindow()
        {
            this.PollyProps = App.PollyProperties;
            this.AudioProps = App.AudioDeviceProperties;
            this.PollyHistory = App.PollyHistory;

            App.LoadSettings();
            
            pc = new PollyCaller(AudioProps);
            
            InitializeComponent();
            
            this.DataContext = this;
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            PlaySound();
        }
        
        private void PlayHotKey_Executed(object sender, ExecutedRoutedEventArgs e) { PlaySound(); }

        private void PlaySound()
        {
            if (string.IsNullOrEmpty(PollyProps.ApiKey) || PollyProps.ApiKey.Contains(","))
            {
                MessageBox.Show("You need to set a Key before you can use the Application","Key Required!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else
            {
                pc.Call(PollyProps);
                PollyHistory.MakeMemento(this.PollyProps);
            }
        }

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
                    pc.SaveToFile(file,PollyProps);
                    PollyHistory.MakeMemento(this.PollyProps);
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    //Do nothing, why would you
                    break;
            }
        }
        
        private void SaveHotKey_Executed(object sender, ExecutedRoutedEventArgs e) { SaveButton_OnClick(sender,e); }

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
        
        /*
         * Called on window close - starts the properties saving in App.
         */
        private void PollyWindow_OnClosing(object sender, CancelEventArgs e)
        {
            App.SaveSettings();
        }


        private void HistoryBackwardButton_OnClick(object sender, RoutedEventArgs e) { MoveHistoryBackward(); }

        private void BackHotKey_Executed(object sender, ExecutedRoutedEventArgs e) { MoveHistoryBackward(); }
        
        /**
         * This method sets the Settings of PollyProps (Text, Voice & Sampling)
         * to the last-seen one.
         * If you are already somewhere in history, it goes one step (further) back.
         * Save-Points are created on "Play" or "Save"
         *
         * It is separated to allow Hotkey and Button usage without duplication.
         */
        private void MoveHistoryBackward()
        {
            if (PollyHistory.HasElements())
            {
                var lastPollyProperties = this.PollyHistory.MoveBack();
                
                this.PollyProps.TextToPlay = lastPollyProperties.TextToPlay;
                this.PollyProps.Voice = lastPollyProperties.Voice;
                this.PollyProps.SamplingRate = lastPollyProperties.SamplingRate;
            }
        }

        private void HistoryForwardButton_OnClick(object sender, RoutedEventArgs e) { MoveHistoryForward(); }

        private void ForwardHotKey_Executed(object sender, ExecutedRoutedEventArgs e) { MoveHistoryForward(); }
        
        /**
         * This method sets the Settings of PollyProps (Text, Voice & Sampling)
         * to the next-to-current-seen one.
         * If you took 1 step back and then 1 step forth, you are at the current played entry.
         * Save-Points are created on "Play" or "Save".
         *
         * It is separated to allow Hotkey and Button usage without duplication.
         */
        private void MoveHistoryForward()
        {
            if (PollyHistory.HasElements())
            {
                var nextPollyProperties = this.PollyHistory.MoveForth();

                this.PollyProps.TextToPlay = nextPollyProperties.TextToPlay;
                this.PollyProps.Voice = nextPollyProperties.Voice;
                this.PollyProps.SamplingRate = nextPollyProperties.SamplingRate;
            }
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            pc.CancelAllRunningSounds();
        }

        private void MuteHotkey_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            pc.CancelAllRunningSounds();
        }
    }
}