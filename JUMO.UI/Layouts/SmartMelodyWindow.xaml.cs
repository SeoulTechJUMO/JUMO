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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SmartMelodyViewModel svm = (SmartMelodyViewModel)DataContext;
            svm.ChangeScore(svm.CurrentMelody, true);
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
