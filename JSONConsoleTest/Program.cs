using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace JSONConsoleTest
{
    class Program
    {
        public static WebClient getAuth(string username, string password)
        {
            var json = new JObject();
            json.Add("username", username);
            json.Add("password", password);

            string uri = "https://api.hooktheory.com/v1/users/auth";
            string requestJson = json.ToString();
            WebClient wc = new WebClient();
            wc.Headers[HttpRequestHeader.Accept] = "application/json";
            wc.Headers[HttpRequestHeader.ContentType] = "application/json";
            wc.Encoding = UTF8Encoding.UTF8;
            string responseJSON = wc.UploadString(uri, requestJson);
            var auth = JObject.Parse(responseJSON);
            wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + (string)auth["activkey"];

            return wc;
        }

        public static JArray makeProgress(WebClient wc)
        {
            string uri = "https://api.hooktheory.com/v1/trends/nodes";
            Stream stream = wc.OpenRead(uri);
            string responseJSON = new StreamReader(stream).ReadToEnd();
            var response = JArray.Parse(responseJSON);

            return response;
        }

        public static JArray makeProgress(WebClient wc, string cp)
        {
            string uri = "https://api.hooktheory.com/v1/trends/nodes?cp="+cp;
            Stream stream = wc.OpenRead(uri);
            string responseJSON = new StreamReader(stream).ReadToEnd();
            var response = JArray.Parse(responseJSON);

            return response;
        }

        public static void Main(string[] args)
        {
            var wc = getAuth("DADPAPA", "operation");
            var progress = makeProgress(wc);
            //Console.WriteLine(makeProgress(wc,(string)progress[1]["child_path"]));
            string cp = "1";
            Console.WriteLine(makeProgress(wc, cp));
        }
    }
}
