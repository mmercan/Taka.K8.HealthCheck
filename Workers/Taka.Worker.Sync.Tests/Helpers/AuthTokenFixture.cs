using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Taka.Worker.Sync.Tests.Helpers
{
    public class AuthTokenFixture : IDisposable
    {
        // private ITestOutputHelper output;
        public AuthTokenFixture()
        {
            //  this.output = output;
            Token = GetToken();
        }



        public string GetToken()
        {
            var appId = Environment.GetEnvironmentVariable("APPID");
            var clientSecret = Environment.GetEnvironmentVariable("APPSECRET");
            var adId = Environment.GetEnvironmentVariable("ADID");

            var url = "https://login.microsoftonline.com/" + adId + "/oauth2/token?resource=" + appId;

            // output.WriteLine("APPID is " + appId);
            // output.WriteLine("APPSECRET is " + clientSecret);
            // output.WriteLine("ADID is " + adId);

            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            nvc.Add(new KeyValuePair<string, string>("client_id", appId));
            nvc.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            nvc.Add(new KeyValuePair<string, string>("resource", appId));
            var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(nvc) };
            var resTask = client.SendAsync(req);
            resTask.Wait();

            var ContentTask = resTask.Result.Content.ReadAsStringAsync();
            ContentTask.Wait();
            //return ContentTask.Result;
            // output.WriteLine("content  " + ContentTask.Result);
            JObject s = JObject.Parse(ContentTask.Result);
            client.Dispose();
            return "Bearer " + (string)s["access_token"];
        }

        public void Dispose()
        {

        }

        public string Token { get; private set; }
    }
}