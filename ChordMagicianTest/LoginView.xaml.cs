using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChordMagicianTest
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var API = new getAPI();
                var progress_list = API.Request(username.Text, password.Password);
                CodeMagicView cm = new CodeMagicView(API,progress_list);
                cm.Show();
                this.Close();
            }
            catch
            {
                MessageBox.Show("로그인 정보가 잘못되었습니다. 다시 입력 해주세요.");
            }
        }

        private void Signup_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }
    }
}
