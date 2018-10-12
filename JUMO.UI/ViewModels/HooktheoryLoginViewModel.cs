using System.Net;
using System.Threading.Tasks;
using ChordMagicianModel;

namespace JUMO.UI
{
    class HooktheoryLoginViewModel : ViewModelBase
    {
        private readonly GetAPI _api;
        private bool _isBusy = false;

        #region Properties

        public override string DisplayName => "Hooktheory에 로그인";

        public string Username { get; set; } = "";

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public WebExceptionStatus LastError => _api.LastError;
        public HttpStatusCode LastStatus => _api.LastStatus;

        public RelayCommand OpenHyperlinkCommand { get; } = new RelayCommand(uri => System.Diagnostics.Process.Start((string)uri));

        #endregion

        public HooktheoryLoginViewModel(GetAPI api)
        {
            _api = api;
        }

        public async Task<bool> TestTokenAsync()
        {
            IsBusy = true;

            await Task.Run(() => _api.GetProgress(""));

            IsBusy = false;

            return _api.LastError == WebExceptionStatus.Success;
        }

        public async Task<bool> SignInAsync(string username, string password)
        {
            IsBusy = true;

            bool signInResult = await Task.Run(() => _api.SignIn(username, password));

            IsBusy = false;

            return signInResult;
        }
    }
}
