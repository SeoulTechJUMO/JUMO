using System.Net;
using System.Windows;
using ChordMagicianModel;
using JUMO.UI.ViewModels;

namespace JUMO.UI.Layouts
{
    public partial class HooktheoryLoginDialog : Window
    {
        private readonly GetAPI _api;
        private readonly HooktheoryLoginViewModel _viewModel;

        public HooktheoryLoginDialog(GetAPI api)
        {
            InitializeComponent();

            _api = api;
            DataContext = _viewModel = new HooktheoryLoginViewModel(_api);
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (await _viewModel.TestTokenAsync())
            {
                OnLoginSuccess();
            }
        }

        private async void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (await _viewModel.SignInAsync(username.Text, password.Password))
            {
                OnLoginSuccess();
            }
            else
            {
                if (_viewModel.LastError == WebExceptionStatus.ProtocolError
                    && _viewModel.LastStatus >= HttpStatusCode.BadRequest
                    && _viewModel.LastStatus < HttpStatusCode.InternalServerError)
                {
                    MessageBox.Show("로그인 정보가 잘못되었습니다. 다시 확인 후 입력해주세요.");
                }
                else if (_viewModel.LastError == WebExceptionStatus.NameResolutionFailure)
                {
                    MessageBox.Show("인터넷 연결을 확인해주세요.");
                }
                else
                {
                    MessageBox.Show("오류가 발생했습니다, 잠시후 다시 시도해주세요.");
                }
            }
        }

        private void OnLoginSuccess()
        {
            DialogResult = true;

            Close();
        }
    }
}
