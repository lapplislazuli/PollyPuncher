namespace PollyPuncher
{
    /*
     * This Class is bound to the Mainwindow (View) to select the possible values
     * It is also used in the PollyCaller (Model)
     */
    public class PollyProperties
    {
        private string apiKey { get; set; }     // Your API Key for Request Signing
        private string voice { get; set; }      // The Voice to use (e.g. German - Hans)
        private string text { get; set; }       // The Text to Synthesize
    }
}