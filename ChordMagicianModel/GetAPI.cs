using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ChordMagicianModel
{
    public class GetAPI
    {
        private const string API_BASE = "https://api.hooktheory.com/v1/";

        private readonly WebClient _clientEx = new WebClient();

        private string _token = "";

        private string Token
        {
            get => _token;
            set
            {
                _token = value;
                _clientEx.Headers[HttpRequestHeader.Authorization] = $"Bearer {_token}";
            }
        }

        public WebExceptionStatus LastError { get; private set; } = WebExceptionStatus.Success;
        public HttpStatusCode LastStatus { get; private set; } = HttpStatusCode.OK;

        public GetAPI()
        {
            // Token = Properties.Settings.Default.Token;

            _clientEx.Encoding = Encoding.UTF8;
            _clientEx.Headers[HttpRequestHeader.Accept] = "application/json";
            _clientEx.Headers[HttpRequestHeader.ContentType] = "application/json";
        }

        public bool SignIn(string username, string password)
        {
            var json = new JObject()
            {
                { "username", username },
                { "password", password }
            };

            try
            {
                string responseText = _clientEx.UploadString(API_BASE + "users/auth", json.ToString());
                JObject responseJson = JObject.Parse(responseText);

                Token = (string)responseJson["activkey"];

                Properties.Settings.Default.username = username;
                Properties.Settings.Default.password = password;
                // Properties.Settings.Default.Token = Token;
                Properties.Settings.Default.Save();

                OnWebSuccess();

                return true;
            }
            catch (WebException e)
            {
                OnWebException(e);

                return false;
            }
        }

        public ObservableCollection<Progress> GetProgress(string childPath)
        {
            try
            {
                string responseText = _clientEx.DownloadString(API_BASE + "trends/nodes?cp=" + childPath);
                JArray responseJson = JArray.Parse(responseText);

                OnWebSuccess();

                return ConvertToProgress(responseJson);
            }
            catch (WebException e)
            {
                OnWebException(e);

                return null;
            }
        }

        private ObservableCollection<Progress> ConvertToProgress(JArray progresses)
        {
            var progress = new ObservableCollection<Progress>();

            foreach (JObject i in progresses)
            {
                var k = new Progress((string)i["chord_ID"], (string)i["chord_HTML"], (double)i["probability"], (string)i["child_path"]);

                progress.Add(k);
            }

            return progress;
        }

        private void OnWebSuccess()
        {
            LastError = WebExceptionStatus.Success;
            LastStatus = HttpStatusCode.OK;
        }

        private void OnWebException(WebException e)
        {
            Debug.WriteLine(e.ToString());

            LastError = e.Status;

            if (e.Response is HttpWebResponse response)
            {
                LastStatus = response.StatusCode;
            }
        }
    }
}
