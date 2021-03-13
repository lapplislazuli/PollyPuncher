namespace PollyPuncher
{
    /*
     * This Class is bound to the Mainwindow (View) to select the possible values
     * It is also used in the PollyCaller (Model)
     */
    public class PollyProperties
    {
        public string apiKey { get; set; }     // Your API Key for Request Signing
        public string voice { get; set; }      // The Voice to use (e.g. German - Hans)
        public string text { get; set; }       // The Text to Synthesize

        public enum voices
        {
            JENNY,
            HANS,
            //TODO: Which ones are actually availible
            DEINEMUDAA
        }
    }
}