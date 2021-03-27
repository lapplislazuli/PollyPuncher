using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PollyPuncher
{
    /*
     * This Class is bound to the Mainwindow (View) to select the possible values
     * It is also used in the PollyCaller (Model)
     */
    public class PollyProperties : ICloneable, INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        private string _apikey = String.Empty; // Your API Key for Request Signing
        public string ApiKey
        {
            get { return _apikey;}
            set {
                if (value != _apikey)
                {
                    this._apikey = value;
                    NotifyPropertyChanged();
                }
            }
        }     

        private string _voice = "Hans"; // The Voice to use (e.g. German - Hans)
        
        public string Voice
        {
            get { return this._voice;}
            set
            {
                if (value != this._voice)
                {
                    this._voice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _textToPlay = "Beispiel Text für fröhliche Menschen.";      // The Text to Synthesize
        public string TextToPlay
        {
            get { return this._textToPlay;}
            set
            {
                if (value != this._textToPlay)
                {
                    this._textToPlay = value;
                    NotifyPropertyChanged();
                }
            } }

        private int _samplingRate = 16000; //Sampling Rate in hz, use either 4k,8k,16k (32k?)
        public int SamplingRate
        {
            get { return this._samplingRate;}
            set {
                if (value != this._samplingRate)
                {
                    this._samplingRate = value;
                    NotifyPropertyChanged();
                }}
        } 


        public object Clone()
        {
            // Cloning is rather easy, as Strings are pass by value (so is the int of sampling rate)
            var clone = new PollyProperties()
            {
                Voice = this.Voice,
                ApiKey = this.ApiKey,
                TextToPlay = this.TextToPlay,
                SamplingRate = this.SamplingRate
            };
            return clone;
        }
        
        /*
         * According to https://docs.aws.amazon.com/polly/latest/dg/voicelist.html
         * All Voices should be in standard except for Kevin And Olivia
         */
        private readonly List<string> _voices = new List<string>()
        {
            "Nicole", 
            //"Kevin",  "Olivia",
            "Enrique", "Tatyana", "Russell", "Lotte", "Geraint", "Carmen", "Mads", "Penelope", "Mia", "Joanna", "Matthew", "Brian", "Seoyeon", "Ruben", "Ricardo",
            "Maxim", "Lea", "Giorgio", "Carla", "Naja", "Maja", "Astrid", "Ivy", "Kimberly", "Chantal", "Amy", "Vicki", "Marlene", "Ewa", "Conchita",
            "Camila", "Karl", "Zeina", "Miguel", "Mathieu", "Justin", "Lucia", "Jacek", "Bianca", "Takumi", "Ines", "Gwyneth", 
            "Cristiano", "Mizuki", "Celine", "Zhiyu", "Jan", "Liv", "Joey", "Raveena", "Filiz", "Dora", "Salli","Aditi","Vitoria", "Emma", "Lupe","Kendra",
            "Hans"
        };
        
        public List<string> Voices
        {
            get => _voices.Select(a => a).ToList();
        }

    }
}