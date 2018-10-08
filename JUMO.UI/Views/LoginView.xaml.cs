using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChordMagicianModel;

namespace JUMO.UI.Views
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginView : Window
    {
        //전달받은 피아노롤 vm
        PianoRollViewModel vm;

        public LoginView(PianoRollViewModel _vm)
        {
            InitializeComponent();
            vm = _vm;
            if (ChordMagicianModel.Properties.Settings.Default.username != "" && ChordMagicianModel.Properties.Settings.Default.password != "")
            {
                try
                {
                    Login(ChordMagicianModel.Properties.Settings.Default.username, ChordMagicianModel.Properties.Settings.Default.password, true);
                }
                catch (Exception e){ System.Diagnostics.Debug.WriteLine(e); }
            }
            username.Focus();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            Login(username.Text, password.Password);
        }

        private void Signup_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        public void Login(string username, string password, bool first=false)
        {
            try
            {
                var API = new GetAPI();
                var progress_list = API.Request(username, password);
                if (AutoLogin.IsChecked == true)
                {
                    ChordMagicianModel.Properties.Settings.Default.username = username;
                    ChordMagicianModel.Properties.Settings.Default.password = password;
                    ChordMagicianModel.Properties.Settings.Default.Save();
                }
                CodeMagicView cm = new CodeMagicView(vm);
                cm.DataContext = new ChordMagicViewModel("C", "Major", API, progress_list, vm);
                cm.Show();
                this.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    if (((HttpWebResponse)e.Response).StatusDescription == "Unauthorized")
                    {
                        MessageBox.Show("로그인 정보가 잘못되었습니다. 다시 확인 후 입력해주세요.");
                    }
                    
                }
                else if (e.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    MessageBox.Show("인터넷 연결을 확인해주세요.");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                if (first != true)
                    MessageBox.Show("오류가 발생했습니다, 잠시후 다시 시도해주세요.");
            }
        }

        private void EnterInput(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Login(username.Text, password.Password);
            }
        }
    }
}
