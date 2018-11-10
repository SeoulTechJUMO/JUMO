using System.Windows;
using JUMO.UI.Controls;

namespace JUMO.UI.Layouts
{
    public partial class SettingsWindow : WindowBase
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void OnOKClick(object sender, RoutedEventArgs e)
        {
            ((SettingsViewModel)DataContext).SaveSettings();
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            ((SettingsViewModel)DataContext).SaveSettings();
        }
    }
}
