using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace PollyPuncher
{
    
    /*
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
        private PollyProperties PollyProps { get; set; }
        private AudioDeviceProperties AudioProps { get; set; }

        private Amazon.Runtime.CredentialManagement.CredentialProfile _awsProfile;
        private int HASHCODELENGTH { get; } = 8;
        private System.IO.DirectoryInfo sound_dir;

        private Dictionary<string, Stream> _knownHashes = new Dictionary<string, Stream>();
        
        public PollyCaller(PollyProperties pollyProps, AudioDeviceProperties audioProps)
        {
            this.PollyProps = pollyProps;
            this.AudioProps = audioProps;

            setupSoundsFolder();
        }

        /**
         * This method creates or cleans a nearby folder "sounds" where the mp3 files are stored temporarily.
         * I decided to clean it on system startup, in case a user wants to look at the files after closing.
         * I did purposefully keep and read it, as otherwise this relative folder will get very crowded after a while
         * and might take up significant space for the user. 
         */
        private void setupSoundsFolder()
        {
            string sound_dir_path = Path.GetRelativePath(".","sounds");
            // Create the "sounds" directory if not exists
            if (!Directory.Exists(sound_dir_path))
            {
                sound_dir = Directory.CreateDirectory(sound_dir_path);
            }
            // If it exists, clean it from all mp3 files of the last run
            else
            {
                sound_dir = new DirectoryInfo(sound_dir_path);
                foreach (FileInfo file in sound_dir.GetFiles())
                {
                    file.Delete(); 
                }
            }
        }
        
        private void setAWSProfile()
        {
            // Read the Credentials and store them shortly in an array
            string path = PollyProps.apiKey;
            
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
        
        public void Call()
        {
            setAWSProfile();
            
            string tempName = temporaryName();
            string tempFilePath = Path.GetFullPath( tempName + ".mp3",sound_dir.FullName);
            
            Stream audioStream = null;
            if (_knownHashes.ContainsKey(tempName))
            {
                audioStream = _knownHashes[tempName];
            }
            else
            {
                audioStream = makeAWSCall();
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
            PlaySound(tempFilePath,AudioProps.deviceA -1);
            if (AudioProps.deviceA != AudioProps.deviceB)
                PlaySound(tempFilePath,AudioProps.deviceB -1 );
        }

        public void SaveToFile(string mp3Filename)
        {
            setAWSProfile();

            string tempName = temporaryName();
            Stream audioStream = null;
            if (_knownHashes.ContainsKey(tempName))
            {
                audioStream = _knownHashes[tempName];
            }
            else
            {
                audioStream = makeAWSCall();
                _knownHashes[tempName] = audioStream;
            }

            using (FileStream fs = File.Create(mp3Filename))
            {
                audioStream.CopyTo(fs);
                fs.Flush();
            }
        }
        
        private Stream makeAWSCall()
        {
            // Store the Amazon Profile in the Credentials Engine for later use
            var netSDKFile = new NetSDKCredentialsFile();
            netSDKFile.RegisterProfile(_awsProfile);
            
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
                        SampleRate = PollyProps.sampling.ToString(),
                        Text = PollyProps.textToPlay,
                        TextType = "text",
                        VoiceId = PollyProps.voice // One of Hans, Jenny , ...
                    });
                    response.Wait();

                    var res = response.Result;
                    audioStream = res.AudioStream;
                }
            }

            return audioStream;
        }

        private void PlaySound(string path, int audioDevice = 0 , Action done = null)
        {
            FileStream ms = File.OpenRead(path);
            var rdr = new Mp3FileReader(ms);
            // TODO: Can I use MemoryStream here?
            WaveStream wavStream = WaveFormatConversionStream.CreatePcmStream(rdr);
            var baStream = new BlockAlignReductionStream(wavStream);
            
            var waveOut = new WaveOut();
            waveOut.DeviceNumber = audioDevice;
            
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
        private string temporaryName()
        {
            string input = PollyProps.textToPlay + "+" + PollyProps.voice + "+" + PollyProps.voice;
            string output;
            using (var provider = System.Security.Cryptography.MD5.Create())
            {
                StringBuilder builder = new StringBuilder();                           

                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(input)))
                    builder.Append(b.ToString("x2").ToLower());

                output = builder.ToString();
            }

            return output.Substring(0, this.HASHCODELENGTH);
        }

    }
    

    
}