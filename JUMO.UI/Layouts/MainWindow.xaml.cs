using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JUMO.UI.Controls;

namespace JUMO.UI.Layouts
{
    public partial class MainWindow : WindowBase
    {
        private static int _octave = 48;

        private static readonly Key[] _upperKeys = new []
        {
            Key.Q, Key.D2, Key.W, Key.D3, Key.E, Key.R, Key.D5, Key.T, Key.D6, Key.Y, Key.D7, Key.U, Key.I, Key.D9, Key.O, Key.D0, Key.P, Key.OemOpenBrackets, Key.OemPlus, Key.OemCloseBrackets
        };

        private static readonly Key[] _lowerKeys = new []
        {
            Key.Z, Key.S, Key.X, Key.D, Key.C, Key.V, Key.G, Key.B, Key.H, Key.N, Key.J, Key.M, Key.OemComma, Key.L, Key.OemPeriod, Key.OemSemicolon, Key.OemQuestion
        };

        private bool[] _upperKeyState = new bool[_upperKeys.Length];
        private bool[] _lowerKeyState = new bool[_lowerKeys.Length];

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

            if ((DataContext as MainViewModel)?.CurrentWorkspace is PianoRollViewModel vm)
            {
                if (HandleVirtualMidiKeyPress(e, vm.Plugin))
                {
                    e.Handled = true;

                    return;
                }
            }
        }

        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (SkipKeyHandling(e))
            {
                return;
            }

            if ((DataContext as MainViewModel)?.CurrentWorkspace is PianoRollViewModel vm)
            {
                if (HandleVirtualMidiKeyRelease(e, vm.Plugin))
                {
                    e.Handled = true;

                    return;
                }
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

        private bool HandleVirtualMidiKeyPress(KeyEventArgs e, Vst.Plugin plugin)
        {
            int idx;

            if ((idx = Array.IndexOf(_upperKeys, e.Key)) >= 0)
            {
                if (!_upperKeyState[idx])
                {
                    _upperKeyState[idx] = true;
                    plugin.NoteOn((byte)(idx + _octave + 12), 100);

                    return true;
                }
            }
            else if ((idx = Array.IndexOf(_lowerKeys, e.Key)) >= 0)
            {
                if (!_lowerKeyState[idx])
                {
                    _lowerKeyState[idx] = true;
                    plugin.NoteOn((byte)(idx + _octave), 100);

                    return true;
                }
            }

            return false;
        }

        private bool HandleVirtualMidiKeyRelease(KeyEventArgs e, Vst.Plugin plugin)
        {
            int idx;

            if ((idx = Array.IndexOf(_upperKeys, e.Key)) >= 0)
            {
                _upperKeyState[idx] = false;
                plugin.NoteOff((byte)(idx + _octave + 12));

                return true;
            }
            else if ((idx = Array.IndexOf(_lowerKeys, e.Key)) >= 0)
            {
                _lowerKeyState[idx] = false;
                plugin.NoteOff((byte)(idx + _octave));

                return true;
            }

            return false;
        }
    }
}
