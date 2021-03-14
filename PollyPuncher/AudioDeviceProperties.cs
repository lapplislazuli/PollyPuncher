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
        public int DeviceA { get; set; }
        public int DeviceB { get; set; }

        public double VolumeA { get; set; } = 50.0; // Volume how loud in %, 0 is silent and 100 is full volume
        public double VolumeB { get; set; } = 50.0; // Volume how loud in %, 0 is silent and 100 is full volume

        private List<DirectSoundDeviceInfo> directSoundDeviceInfos
        {
            get =>  DirectSoundOut.Devices.ToList();
        } 

        public List<string> DeviceNames
        {
            get => directSoundDeviceInfos.Select(a => a.Description).ToList();
        }

    }
}