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
            "Jenny",
            "Hans"
        };
        
        public List<string> Voices
        {
            get => _voices.Select(a => a).ToList();
        }

    }
}