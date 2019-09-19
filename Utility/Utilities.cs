using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Kestavar.Utility;

namespace Kestavar
{
    public class Utilities
    {
        public static string GenerateHash(string Signature)
        {
         
           // string Signature = AgentId + AgentKey + EmailAddress;

            string HashedSignature = Utilities.GenerateSHA512String(Signature);

            return HashedSignature;
        }

        public async static Task<String>GetSessionId(IOptions<MyConfig> config)
        {
            string agentId = config.Value.AgentId;
            string agentKey = config.Value.AgentKey;
            string signature = Utilities.GenerateHash(agentId+agentKey+config.Value.AgentEmail);
            string path = $"paelyt/AccountTransact/GetSessionId?agentid={agentId}&agentkey={agentKey}&signature={signature}";
            var response = await Utilities.MakeCallGet(path);
            var res = JsonConvert.DeserializeObject<SessionResponse>(await response.Content.ReadAsStringAsync());
            return res.sessionid;
        }



        public async static Task<HttpResponseMessage>  MakeCallGet(string path)
        {
        
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://prod.payorboro.com/api/v1/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.Timeout = TimeSpan.FromSeconds(29000);
            HttpResponseMessage response = await client.GetAsync(path);
            return response;
        }

        public async static Task<HttpResponseMessage> MakeCallPost(string path, IOptions<MyConfig> config, HttpContent content, String SessionId)
        {
            HttpClient client = new HttpClient();
            //var SessionId = await GetSessionId(config);
           
            client.BaseAddress = new Uri("http://prod.payorboro.com/api/v1/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("agentID", config.Value.AgentId);
            client.DefaultRequestHeaders.Add("agentKey", config.Value.AgentKey);
            client.DefaultRequestHeaders.Add("sessionid", SessionId);
            client.DefaultRequestHeaders.Add("signature", Utilities.GenerateHash(config.Value.AgentId + config.Value.AgentKey + config.Value.AgentEmail));
            var response = await client.PostAsync(path, content);
           
            return response;

        }


        public string Test(string a, string b)
        {
            return String.Concat(a, b);
        }





        public static string GenerateSHA512String(string inputString)
        {
            SHA512 sha512 = SHA512Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
    public class MyConfig
    {
        public string BaseUrl { get; set; }
        public string AgentId { get; set; }
        public string AgentKey { get; set; }
        public string AgentEmail { get; set; }
    }
}
