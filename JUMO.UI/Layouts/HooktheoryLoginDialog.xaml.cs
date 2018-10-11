using System.Windows;
using System.Net;

namespace JUMO.UI.Layouts
{
    public partial class HooktheoryLoginDialog : Window
    {
        public HooktheoryLoginDialog()
        {
            InitializeComponent();
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            HooktheoryLoginViewModel vm = (HooktheoryLoginViewModel)DataContext;

            if (await vm.TestTokenAsync())
            {
                OnLoginSuccess();
            }
        }

        private async void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            HooktheoryLoginViewModel vm = (HooktheoryLoginViewModel)DataContext;

            if (await vm.SignInAsync(username.Text, password.Password))
            {
                OnLoginSuccess();
            }
            else
            {
                if (vm.LastError == WebExceptionStatus.ProtocolError
                    && vm.LastStatus >= HttpStatusCode.BadRequest
                    && vm.LastStatus < HttpStatusCode.InternalServerError)
                {
                    MessageBox.Show("로그인 정보가 잘못되었습니다. 다시 확인 후 입력해주세요.");
                }
                else if (vm.LastError == WebExceptionStatus.NameResolutionFailure)
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
