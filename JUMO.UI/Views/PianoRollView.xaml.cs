using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using JUMO.UI.Controls;
using JUMO.UI.Layouts;
using JUMO.UI.ViewModels;

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for PianoRollView.xaml
    /// </summary>
    public partial class PianoRollView : UserControl
    {
        private int Octave = 48;

        List<Key> UpKeyboardKeys = new List<Key>
        {
            Key.Q, Key.D2, Key.W, Key.D3, Key.E, Key.R, Key.D5, Key.T, Key.D6, Key.Y, Key.D7, Key.U, Key.I, Key.D9, Key.O, Key.D0, Key.P, Key.OemOpenBrackets, Key.OemPlus, Key.OemCloseBrackets
        };

        List<Key> DownKeyboardKeys = new List<Key>
        {
            Key.Z, Key.S, Key.X, Key.D, Key.C, Key.V, Key.G, Key.B, Key.H, Key.N, Key.J, Key.M, Key.OemComma, Key.L, Key.OemPeriod, Key.OemSemicolon, Key.OemQuestion
        };

        private bool[] UpKeyState;
        private bool[] DownKeyState;

        public PianoRollView()
        {
            UpKeyState = new bool[UpKeyboardKeys.Count];
            DownKeyState = new bool[DownKeyboardKeys.Count];
            InitializeComponent();
        }

        private void PianoRollKeyboard_KeyPressed(object sender, PianoRollKeyEventArgs e)
        {
            (DataContext as PianoRollViewModel).Plugin.NoteOn(e.NoteValue, e.Velocity);
            System.Diagnostics.Debug.WriteLine($"PianoRollView::PianoRollKeyboard_KeyPressed value = {e.NoteValue}, velocity = {e.Velocity}");
        }

        private void PianoRollKeyboard_KeyReleased(object sender, PianoRollKeyEventArgs e)
        {
            (DataContext as PianoRollViewModel).Plugin.NoteOff(e.NoteValue);
            System.Diagnostics.Debug.WriteLine($"PianoRollView::PianoRollKeyboard_KeyReleased value = {e.NoteValue}, velocity = {e.Velocity}");
        }

        private void PianoRollCanvas_AddNoteRequested(object sender, AddNoteRequestedEventArgs e)
        {
            ((PianoRollViewModel)DataContext).AddNote(e.Note);
        }

        private void PianoRollCanvas_DeleteNoteRequested(object sender, DeleteNoteRequestedEventArgs e)
        {
            PianoRollViewModel vm = (PianoRollViewModel)DataContext;

            foreach (Note note in e.NotesToDelete)
            {
                vm.RemoveNote(note);
            }
        }

        private void PianoRollCanvas_SelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PianoRollViewModel vm = (PianoRollViewModel)DataContext;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    vm.SelectItems(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    vm.DeselectItems(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    vm.ClearSelection();
                    break;
            }
        }

        private void MusicalCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                PianoRollViewModel vm = (PianoRollViewModel)DataContext;
                RelayCommand zoomCmd = e.Delta > 0 ? vm.ZoomInCommand : vm.ZoomOutCommand;

                if (zoomCmd.CanExecute(null))
                {
                    zoomCmd.Execute(null);
                }

                e.Handled = true;
            }
        }

        private void ExcuteChordMagician(object sender, RoutedEventArgs e)
        {
            new ChordMagicianWindow((PianoRollViewModel)DataContext).Show();
        }

        private void NoteToolboxButtonClick(object sender, RoutedEventArgs e)
        {
            Toolbox.ContextMenu.IsOpen = true;
        }

        private void SofterOpen(object sender, RoutedEventArgs e)
        {
            SofterViewModel ntvm = new SofterViewModel((PianoRollViewModel)DataContext);
            NoteSofterWindow nsv = new NoteSofterWindow { DataContext = ntvm };

            nsv.Show();
        }

        private void ChopperOpen(object sender, RoutedEventArgs e)
        {
            ChopperViewModel ntvm = new ChopperViewModel((PianoRollViewModel)DataContext);
            NoteChopperWindow ncv = new NoteChopperWindow { DataContext = ntvm };

            ncv.Show();
        }

        private void KeyPress(object sender, KeyEventArgs e)
        {
            if (UpKeyboardKeys.Contains(e.Key))
            {
                int idx = UpKeyboardKeys.IndexOf(e.Key);
                if (!UpKeyState[idx])
                {
                    UpKeyState[idx] = true;
                    (DataContext as PianoRollViewModel).Plugin.NoteOn((byte)(UpKeyboardKeys.IndexOf(e.Key) + Octave + 12), 100);
                }
            }
            else if(DownKeyboardKeys.Contains(e.Key))
            {
                int idx = DownKeyboardKeys.IndexOf(e.Key);
                if (!DownKeyState[idx])
                {
                    DownKeyState[idx] = true;
                    (DataContext as PianoRollViewModel).Plugin.NoteOn((byte)(DownKeyboardKeys.IndexOf(e.Key) + Octave), 100);
                }
            }
        }

        private void KeyRelease(object sender, KeyEventArgs e)
        {
            if (UpKeyboardKeys.Contains(e.Key))
            {
                int idx = UpKeyboardKeys.IndexOf(e.Key);
                UpKeyState[idx] = false;
                (DataContext as PianoRollViewModel).Plugin.NoteOff((byte)(UpKeyboardKeys.IndexOf(e.Key) + Octave + 12));
            }
            else if(DownKeyboardKeys.Contains(e.Key))
            {
                int idx = DownKeyboardKeys.IndexOf(e.Key);
                DownKeyState[idx] = false;
                (DataContext as PianoRollViewModel).Plugin.NoteOff((byte)(DownKeyboardKeys.IndexOf(e.Key) + Octave));
            }
        }
    }
}
