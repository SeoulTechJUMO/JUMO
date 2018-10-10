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

namespace JUMO.UI.Views
{
    /// <summary>
    /// SmartMelodyView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmartMelodyView : Window
    {
        public SmartMelodyView()
        {
            InitializeComponent();
        }

        private void InsertButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
