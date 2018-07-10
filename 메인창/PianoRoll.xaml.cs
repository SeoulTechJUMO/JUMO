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

namespace 메인창
{
    /// <summary>
    /// PlayList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PianoRoll : Window
    {
        public PianoRoll()
        {
            InitializeComponent();

            TextBlock text1 = new TextBlock();
            
        }

        private void PianoRoll_Load(object sender, EventArgs e)
        {
            text1.Text = "포인터";
        }

        private void instBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
