using System.Collections.Generic;
using System.Linq;

namespace PollyPuncher
{
    /*
     * This Class is bound to the Mainwindow (View) to select the possible values
     * It is also used in the PollyCaller (Model)
     */
    public class PollyProperties 
    {
        public string ApiKey { get; set; }     // Your API Key for Request Signing
        public string Voice { get; set; } = "Hans";    // The Voice to use (e.g. German - Hans)
        public string TextToPlay { get; set; } = "Beispiel Text für fröhliche Menschen.";      // The Text to Synthesize

        public int SamplingRate { get; set; } = 16000; //Sampling Rate in hz, use either 4k,8k,16k,32k

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