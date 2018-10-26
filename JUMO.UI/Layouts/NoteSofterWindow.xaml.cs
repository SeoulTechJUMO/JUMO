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

namespace JUMO.UI.Layouts
{
    /// <summary>
    /// NoteSofterWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NoteSofterWindow : Window
    {
        public NoteSofterWindow()
        {
            InitializeComponent();
        }

        private void CheckButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
