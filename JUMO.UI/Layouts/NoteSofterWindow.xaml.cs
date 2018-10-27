using System.Windows;
using JUMO.UI.ViewModels;

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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            NoteToolsViewModel nvm = (NoteToolsViewModel)DataContext;
            if (!nvm.WillInsert) { nvm.Reset(); }
        }

        private void CheckButtonClick(object sender, RoutedEventArgs e)
        {
            NoteToolsViewModel nvm = (NoteToolsViewModel)DataContext;
            nvm.WillInsert = true;
            Close();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
    }
}
