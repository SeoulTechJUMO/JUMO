using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JUMO.UI.Controls;

namespace JUMO.UI.Layouts
{
    public partial class MainWindow : WindowBase
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }

        private void Mixer_Click(object sender, RoutedEventArgs e)
        {
            new MixerWindow().Show();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (SkipKeyHandling(e))
            {
                return;
            }

            if (e.Key == Key.Space)
            {
                ICommand cmd = (DataContext as MainViewModel)?.TogglePlaybackCommand;

                if (cmd?.CanExecute(null) ?? false)
                {
                    cmd?.Execute(null);
                }

                e.Handled = true;

                return;
            }
        }

        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (SkipKeyHandling(e))
            {
                return;
            }
        }

        private bool SkipKeyHandling(KeyEventArgs e)
        {
            IInputElement focused = Keyboard.FocusedElement;

            if (Keyboard.Modifiers != ModifierKeys.None
                || focused is TextBox
                || focused is MenuItem
                || focused is NumericUpDown)
            {
                e.Handled = false;

                return true;
            }

            if (e.IsRepeat)
            {
                e.Handled = true;

                return true;
            }

            return false;
        }
    }
}
