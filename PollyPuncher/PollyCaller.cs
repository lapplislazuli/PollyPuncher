using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Threading;
using System.Windows.Automation;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
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
        
        public PollyCaller(PollyProperties pollyProps, AudioDeviceProperties audioProps)
        {
            this.PollyProps = pollyProps;
            this.AudioProps = audioProps;
            
            // TODO: Set Uwi Config
            //testAmazonKit();
        
        }

        public void Call(String text)
        {
            
        }

        /*
         * Read from the Polly Properties and configure the Amazon Client with it
         */
        public void configurePollyClient()
        {
            AmazonPollyConfig pollyConfig = new AmazonPollyConfig();
            
            //_client = new AmazonPollyClient(){Config=pollyConfig};
        }

        public void testAmazonKit()
        {
            
            // Read the Credentials and store them shortly in an array
            var path = @"C:\Users\lguts\Code\PollyPuncher\rootkey.csv";
            
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
            // Store the Amazon Profile in the Credentials Engine for later use
            var netSDKFile = new NetSDKCredentialsFile();
            netSDKFile.RegisterProfile(profile);
            
            // This chain uses the credentials to create a token for usage, 
            // Later the token is used in the Credentials
            // The chain stores it into awsCredentials, that is used for the client.
            var chain = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials;
            // use awsCredentials
            if (chain.TryGetAWSCredentials("polly_profile", out awsCredentials))
            {
                using (var client = new AmazonPollyClient(awsCredentials, profile.Region))
                {
                    var response = client.SynthesizeSpeechAsync(new SynthesizeSpeechRequest 
                    {
                        OutputFormat = "mp3",
                        SampleRate = "8000",
                        Text = "Ich bin der wütende Unterbrecher Hans. ",
                        TextType = "text",
                        VoiceId = "Hans" // One of Hans, Jenny , ...
                    });
                    response.Wait();

                    var res = response.Result;
            
                    var audioStream = res.AudioStream;
                    string contentType = res.ContentType;
                    int requestCharacters = res.RequestCharacters;
                    
                    
                    using (FileStream fs = File.Create("tmp.mp3"))
                    {
                        audioStream.CopyTo(fs);
                        fs.Flush();
                    }

                    var devices = WaveOut.DeviceCount;
                    var directSoundDeviceInfos =  DirectSoundOut.Devices;

                    PlaySound("tmp.mp3",0);
                    PlaySound("tmp.mp3", 2);
                    //PlaySound(audioStream,0);
                    
                    // For debugging
                    int waiter = 1;
                }
            }

        }
        public void PlaySound(string path, int audioDevice = 0 , Action done = null)
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
        //TODO: Figure this out
        /*
        public void PlaySound(Stream audioStream, int audioDevice = 0 , Action done = null)
        {
            ReadFullyStream(audioStream);
            WaveStream wavStream = WaveFormatConversionStream.CreatePcmStream(audioStream);
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
        */
    }
    
    
}