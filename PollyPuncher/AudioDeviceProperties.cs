using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;

namespace PollyPuncher
{
    
    /*
     * This class is intended to lookup the Audio Devices selectable,
     * And bind it into the MainWindow (Model).
     */
    public class AudioDeviceProperties
    {
        //TODO: Add the selection of the Audio Properties
        public int deviceA { get; set; }
        public int deviceB { get; set; }

        public double volumeA { get; set; } = 50.0; // Volume how loud in %, 0 is silent and 100 is full volume
        public double volumeB { get; set; } = 50.0; // Volume how loud in %, 0 is silent and 100 is full volume

        private List<DirectSoundDeviceInfo> directSoundDeviceInfos
        {
            get =>  DirectSoundOut.Devices.ToList();
        } 

        public List<string> deviceNames
        {
            get => directSoundDeviceInfos.Select(a => a.Description).ToList();
            private set { }
        }

    }
}