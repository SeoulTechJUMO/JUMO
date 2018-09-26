using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace ChordMagicianModel
{
    public class getAPI
    {
        public WebClient wc;

        public WebClient getAuth(string username, string password)
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

        public JArray makeProgress(WebClient wc)
        {
            string uri = "https://api.hooktheory.com/v1/trends/nodes";
            Stream stream = wc.OpenRead(uri);
            string responseJSON = new StreamReader(stream).ReadToEnd();
            var response = JArray.Parse(responseJSON);

            return response;
        }

        public JArray makeProgress(WebClient wc, string cp)
        {
            string uri = "https://api.hooktheory.com/v1/trends/nodes?cp=" + cp;
            Stream stream = wc.OpenRead(uri);
            string responseJSON = new StreamReader(stream).ReadToEnd();
            var response = JArray.Parse(responseJSON);

            return response;
        }

        public ObservableCollection<Progress> ConvertToProgress(JArray JProgress)
        {
            var progress = new ObservableCollection<Progress>();

            foreach (JObject i in JProgress)
            {
                var k = new Progress((string)i["chord_ID"], (string)i["chord_HTML"],(double)i["probability"],(string)i["child_path"]);
                progress.Add(k);
            }

            return progress;
        }

        public ObservableCollection<Progress> Request(string username, string password)
        {
            wc = getAuth(username, password);
            var progress = makeProgress(wc);
            var progress_list = ConvertToProgress(progress);
            return progress_list;
        }

        public ObservableCollection<Progress> Request(string cp)
        {
            var progress = makeProgress(wc, cp);
            var progress_list = ConvertToProgress(progress);
            return progress_list;
        }

        public void show_list(ObservableCollection<Progress> progress_list)
        {
            foreach (var i in progress_list)
            {
                Debug.WriteLine("==================");
                Debug.WriteLine(i);
                Debug.WriteLine("==================");
            }
        }
    }
}
