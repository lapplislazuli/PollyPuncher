using System.Collections;
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
        
        List<DirectSoundDeviceInfo> directSoundDeviceInfos =  DirectSoundOut.Devices.ToList();
    }
}