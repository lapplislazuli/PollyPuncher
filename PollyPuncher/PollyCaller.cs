using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using NAudio.Wave;

namespace PollyPuncher
{
    
    /*
     * Further Reading
     * 
     * Amazon Polly API Reference
     * https://docs.aws.amazon.com/polly/latest/dg/API_Reference.html
     *
     * Amazon Reference on how to register
     * https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-creds.html
     * 
     * More Info
     * https://docs.aws.amazon.com/sdkfornet/v3/apidocs/index.html?page=Polly/MPollySynthesizeSpeechSynthesizeSpeechRequest.html
     */
    
    public class PollyCaller
    {
        private AudioDeviceProperties AudioProps { get; set; }

        private Amazon.Runtime.CredentialManagement.CredentialProfile _awsProfile;
        private const int HashCodeLength = 8;
        private System.IO.DirectoryInfo _soundDir;

        private readonly Dictionary<string, Stream> _knownHashes = new Dictionary<string, Stream>();
        
        public PollyCaller(PollyProperties pollyProps, AudioDeviceProperties audioProps)
        {
            this.AudioProps = audioProps;

            SetupSoundsFolder();
        }

        /**
         * This method creates or cleans a nearby folder "sounds" where the mp3 files are stored temporarily.
         * I decided to clean it on system startup, in case a user wants to look at the files after closing.
         * I did purposefully keep and read it, as otherwise this relative folder will get very crowded after a while
         * and might take up significant space for the user. 
         */
        private void SetupSoundsFolder()
        {
            string soundDirPath = Path.GetRelativePath(".","sounds");
            // Create the "sounds" directory if not exists
            if (!Directory.Exists(soundDirPath))
            {
                _soundDir = Directory.CreateDirectory(soundDirPath);
            }
            // If it exists, clean it from all mp3 files of the last run
            else
            {
                _soundDir = new DirectoryInfo(soundDirPath);
                foreach (var file in _soundDir.GetFiles())
                {
                    file.Delete(); 
                }
            }
        }
        
        /**
         * This method reads the key specified in PollyProperties and sets it as the new AWS profile.
         * At the moment, the region is always set to EU which is something you might want to care for.
         * Except for setting up the account, it is not used or verified at this point.
         * The method only fails if there is something wrong with the KeyFile.
         */
        private void SetAwsProfile(PollyProperties pollyProps)
        {
            // Read the Credentials and store them shortly in an array
            string path = pollyProps.ApiKey;
            
            var credentialFile = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(path))
                credentialFile.Add(row.Split('=')[0], string.Join("=",row.Split('=').Skip(1).ToArray()));
            
            // Create Amazon Credentials
            var options = new CredentialProfileOptions
            {
                AccessKey =  credentialFile["AWSAccessKeyId"],
                SecretKey =  credentialFile["AWSSecretKey"]
            };
            // Make Amazon Credential profile 
            var profile = new Amazon.Runtime.CredentialManagement.CredentialProfile("polly_profile", options);
            profile.Region = RegionEndpoint.EUCentral1;

            this._awsProfile = profile;
        }
        
        /*
         * This method reads the polly properties and audioproperties,
         * then it checks if the wanted combination of text+voice is known already.
         * If the text is known, it will be played to the selected audiodevices,
         * otherwise it will be created by Amazon, stored locally and then played.
         */
        public void Call(PollyProperties pollyProps)
        {
            SetAwsProfile(pollyProps);
            
            string tempName = DeriveTemporaryName(pollyProps);
            string tempFilePath = Path.GetFullPath( tempName + ".mp3",_soundDir.FullName);
            
            Stream audioStream = null;
            if (_knownHashes.ContainsKey(tempName))
            {
                audioStream = _knownHashes[tempName];
            }
            else
            {
                audioStream = MakeAwsCall(pollyProps);
                _knownHashes[tempName] = audioStream;
            }
            
            if(!File.Exists(tempFilePath)){
                /*TODO: Without the existence check, the line below throws a marshal-error 
                /*The file-handle does not seem to be closed properly.
                */
                using (FileStream fs = File.Create(tempFilePath))
                {
                    audioStream.CopyTo(fs);
                    fs.Flush();
                }
            }
        
            // The audio-devices must be decremented by one, as the "default" for NAudio is -1 while its 0 for Windows.
            PlaySound(tempFilePath,AudioProps.DeviceA -1 , AudioProps.VolumeA);
            // Just play the second audio if there is a different selected
            if (AudioProps.DeviceA != AudioProps.DeviceB)
                PlaySound(tempFilePath,AudioProps.DeviceB -1 , AudioProps.VolumeB);
        }
        
        /*
         * This method reads the polly properties and audioproperties,
         * then it checks if the wanted combination of text+voice is known already.
         * If the text is known, it will be saved to the given filename,
         * otherwise it will be created by Amazon, stored locally and then saved.
         */
        public void SaveToFile(string mp3Filename,PollyProperties pollyProps)
        {
            // There was an issue, when the stream was already played. 
            // If that was the case, the stream was "at an end" and could not be saved somewhere (file was just empty). 
            // So if the sound was played, there should be a temp file for it which we just move. 
            // If the file is not there - remove the key and run again.
            
            SetAwsProfile(pollyProps);
            string tempName = DeriveTemporaryName(pollyProps);
            string tempFilePath = Path.GetFullPath( tempName + ".mp3",_soundDir.FullName);
            
            if (_knownHashes.ContainsKey(tempName))
            {
                if (File.Exists(tempFilePath)){
                    File.Copy(tempFilePath, mp3Filename);
                }
                else
                {
                    // The file got lost - thats bad. 
                    // Remove the key, and re-run again. This will also create the file.
                    _knownHashes.Remove(tempName);
                    SaveToFile(mp3Filename,pollyProps);
                }
            }
            else // The hash is not known - create it and make the file + temp file
            {
                Stream audioStream = MakeAwsCall(pollyProps);
                _knownHashes[tempName] = audioStream;
                
                if(File.Exists(mp3Filename))
                    File.Delete(mp3Filename);
                
                using (FileStream fs = File.Create(mp3Filename)) 
                using (FileStream tfs = File.Create(tempFilePath))
                {
                    audioStream.CopyTo(fs);
                    audioStream.CopyTo(tfs);
                    fs.Flush();
                    tfs.Flush();
                }
            }


        }
        
        /*
         * This method reads the polly properties and the ready-made AWS Profile to send a polly request.
         * It returns the Stream of the MP3 created.
         *
         * This method will likely fail in three ways:
         * 1) The Profile is bad
         * 2) The user is offline
         * 3) The specified content for the request is faulty
         *
         * I tried to address the 3) with value checks in the frontend, the others are not checked for atm.
         */
        private Stream MakeAwsCall(PollyProperties pollyProps)
        {
            // Store the Amazon Profile in the Credentials Engine for later use
            var netSdkFile = new NetSDKCredentialsFile();
            netSdkFile.RegisterProfile(_awsProfile);
            
            // This chain uses the credentials to create a token for usage, 
            // Later the token is used in the Credentials
            // The chain stores it into awsCredentials, that is used for the client.
            var chain = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials;
            Stream audioStream = null;
            // use awsCredentials
            if (chain.TryGetAWSCredentials("polly_profile", out awsCredentials))
            {
                using (var client = new AmazonPollyClient(awsCredentials, _awsProfile.Region))
                {
                    var response = client.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
                    {
                        OutputFormat = "mp3",
                        SampleRate = pollyProps.SamplingRate.ToString(),
                        Text = pollyProps.TextToPlay,
                        TextType = "text",
                        VoiceId = pollyProps.Voice // One of Hans, Jenny , ...
                    });
                    response.Wait();

                    var res = response.Result;
                    audioStream = res.AudioStream;
                }
            }

            return audioStream;
        }

        /*
         * Plays the sound found at @path to the @audioDevice with @audioVolume.
         * The default AudioDevice (system standard) is -1.
         * The audio ranges from 0.00 to 1.00, values above 1 will be used as 1.
         * The file under path must be a .mp3 file.
         *
         * This method starts a background worker to play the sound.
         * Playing multiple sounds at once is possible. 
         */
        private void PlaySound(string path, int audioDevice = 0, double audioVolume=50.0, Action done = null)
        {
            FileStream ms = File.OpenRead(path);
            var rdr = new Mp3FileReader(ms);
            // TODO: Can I use MemoryStream here?
            WaveStream wavStream = WaveFormatConversionStream.CreatePcmStream(rdr);
            var baStream = new BlockAlignReductionStream(wavStream);
            
            var waveOut = new WaveOut();
            waveOut.DeviceNumber = audioDevice;
            waveOut.Volume = (float) audioVolume / 100;
            
            waveOut.Init(baStream);
            waveOut.Play();
            
            var bw = new BackgroundWorker();
            bw.DoWork += (s, o) =>
            {
                while (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(150);
                }
                waveOut.Dispose();
                baStream.Dispose();
                wavStream.Dispose();
                rdr.Dispose();
                ms.Dispose();
                if (done != null) done();
            };
            bw.RunWorkerAsync();
        }    
        
        /**
         * This method reads the polly props and creates a hashcode for it.
         * The used hash-method is MD5, which gets cut down to 8 digits.
         * The number of digits is at the moment hardcoded in the PollyCaller as a constant.
         * The hashed properties are Text, Voice & SamplingRate.
         */
        private string DeriveTemporaryName(PollyProperties pollyProps)
        {
            var input = pollyProps.TextToPlay + "+" + pollyProps.Voice + "+" + pollyProps.SamplingRate;
            string output;
            using (var provider = System.Security.Cryptography.MD5.Create())
            {
                var builder = new StringBuilder();
                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(input)))
                    builder.Append(b.ToString("x2").ToLower());
                output = builder.ToString();
            }

            return output.Substring(0, PollyCaller.HashCodeLength);
        }

    }
    

    
}