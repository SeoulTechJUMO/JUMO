using System.Windows;

namespace JUMO.UI.Layouts
{
    /// <summary>
    /// SmartMelodyView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmartMelodyWindow : Window
    {
        public SmartMelodyWindow()
        {
            InitializeComponent();
        }

        private void InsertButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
