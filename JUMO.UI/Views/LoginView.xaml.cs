using System.Windows;
using System.Net;

namespace JUMO.UI.Views
{
    public partial class LoginView : Window
    {
        //전달받은 피아노롤 vm
        PianoRollViewModel vm;

        public LoginView(PianoRollViewModel _vm)
        {
            InitializeComponent();
            vm = _vm;
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            HooktheoryLoginViewModel vmEx = (HooktheoryLoginViewModel)DataContext;

            if (await vmEx.TestTokenAsync())
            {
                ShowChordMagicianWindow();
            }
        }

        private async void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            HooktheoryLoginViewModel vmEx = (HooktheoryLoginViewModel)DataContext;

            if (await vmEx.SignInAsync(username.Text, password.Password))
            {
                ShowChordMagicianWindow();
            }
            else
            {
                if (vmEx.LastError == WebExceptionStatus.ProtocolError
                    && vmEx.LastStatus >= HttpStatusCode.BadRequest
                    && vmEx.LastStatus < HttpStatusCode.InternalServerError)
                {
                    MessageBox.Show("로그인 정보가 잘못되었습니다. 다시 확인 후 입력해주세요.");
                }
                else if (vmEx.LastError == WebExceptionStatus.NameResolutionFailure)
                {
                    MessageBox.Show("인터넷 연결을 확인해주세요.");
                }
                else
                {
                    MessageBox.Show("오류가 발생했습니다, 잠시후 다시 시도해주세요.");
                }
            }
        }

        private void ShowChordMagicianWindow()
        {
            // TODO: 재설계!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ChordMagicianModel.GetAPI api = new ChordMagicianModel.GetAPI();
            var progressList = api.GetProgress("");

            new CodeMagicView(vm)
            {
                DataContext = new ChordMagicViewModel("C", "Major", api, progressList, vm)
            }.Show();

            Close();
        }
    }
}
