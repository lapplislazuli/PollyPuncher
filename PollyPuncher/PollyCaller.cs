using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Automation;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

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

        private AmazonPollyClient _client;
        
        public PollyCaller(PollyProperties pollyProps, AudioDeviceProperties audioProps)
        {
            this.PollyProps = pollyProps;
            this.AudioProps = audioProps;
            
            // TODO: Set Uwi Config
            testAmazonKit();
            
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
                        //LexiconNames = new List<string> {
                        //    "example"
                        //},
                        OutputFormat = "mp3",
                        SampleRate = "8000",
                        Text = "All Gaul is divided into three parts",
                        TextType = "text",
                        VoiceId = "Joanna"
                    });
                    response.Wait();

                    var res = response.Result;
            
                    //MemoryStream audioStream = (MemoryStream) res.AudioStream;
                    string contentType = res.ContentType;
                    int requestCharacters = res.RequestCharacters;
                    
                    // For debugging
                    int waiter = 1;
                }
            }
            
        }
        
    }
    
    
}