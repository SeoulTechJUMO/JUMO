using System.Windows;
using JUMO.UI.ViewModels;

namespace JUMO.UI.Layouts
{
    /// <summary>
    /// NoteChopperWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NoteChopperWindow : Window
    {
        public NoteChopperWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ((NoteToolsViewModel)DataContext).Abort();
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
