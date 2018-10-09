using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChordMagicianModel;

namespace JUMO.UI
{
    class HooktheoryLoginViewModel : ViewModelBase
    {
        private readonly GetAPI _api = new GetAPI();

        public override string DisplayName => "Hooktheory에 로그인";

        public string Username { get; set; } = "";
        public bool StaySignedIn { get; set; } = false;

        public WebExceptionStatus LastError => _api.LastError;
        public HttpStatusCode LastStatus => _api.LastStatus;

        public bool TestTokenAsync()
        {
            _api.GetProgress("");

            return _api.LastError == WebExceptionStatus.Success;
        }

        public bool SignInAsync(string username, string password)
            => _api.SignIn(username, password);
    }
}
