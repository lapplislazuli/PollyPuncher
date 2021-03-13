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
        public string apiKey { get; set; }     // Your API Key for Request Signing
        public string voice { get; set; } = "Hans";    // The Voice to use (e.g. German - Hans)
        public string textToPlay { get; set; } = "Beispiel Text für fröhliche Menschen.";      // The Text to Synthesize

        private List<string> _voices = new List<string>()
        {
            "Nicole", 
            //"Kevin", 
            //"Enrique", "Tatyana", "Russell", "Olivia", "Lotte", "Geraint", "Carmen", "Mads", "Penelope", "Mia", "Joanna", "Matthew", "Brian", "Seoyeon", "Ruben", "Ricardo",
            //"Maxim", "Lea", "Giorgio", "Carla", "Naja", "Maja", "Astrid", "Ivy", "Kimberly", "Chantal", "Amy", "Vicki", "Marlene", "Ewa", "Conchita",
            //"Camila", "Karl", "Zeina", "Miguel", "Mathieu", "Justin", "Lucia", "Jacek", "Bianca", "Takumi", "Ines", "Gwyneth", 
            //"Cristiano", "Mizuki", "Celine", "Zhiyu", "Jan", "Liv", "Joey", "Raveena", "Filiz", "Dora", "Salli","Aditi","Vitoria", "Emma", "Lupe","Kendra",
            "Lea",
            "Carmen",
            "Hans"
        };
        
        public List<string> Voices
        {
            get => _voices.Select(a => a).ToList();
        }

    }
}