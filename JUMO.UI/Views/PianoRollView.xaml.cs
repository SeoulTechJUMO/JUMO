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
using System.Windows.Navigation;
using System.Windows.Shapes;
using JUMO.UI.Controls;

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for PianoRollView.xaml
    /// </summary>
    public partial class PianoRollView : UserControl
    {
        public PianoRollView()
        {
            InitializeComponent();
        }

        private void MusicalCanvas_ZoomChanged(object sender, MusicalCanvasZoomEventArgs e)
        {
            ((PianoRollViewModel)DataContext).ZoomFactor += e.Delta;
        }

        private void PianoRollCanvas_AddNoteRequested(object sender, AddNoteRequestedEventArgs e)
        {
            PianoRollViewModel vm = (PianoRollViewModel)DataContext;

            vm.Notes.Add(new Note(e.Value, 64, e.SnappedPosition, 480));
        }

        private void PianoRollKeyboard_KeyPressed(object sender, PianoRollKeyEventArgs e)
        {
            (DataContext as PianoRollViewModel).Plugin.NoteOn(e.NoteValue, e.Velocity);
            System.Diagnostics.Debug.WriteLine($"PianoRollView::PianoRollKeyboard_KeyPressed value = {e.NoteValue}, velocity = {e.Velocity}");
        }

        private void PianoRollKeyboard_KeyReleased(object sender, PianoRollKeyEventArgs e)
        {
            (DataContext as PianoRollViewModel).Plugin.NoteOff(e.NoteValue, e.Velocity);
            System.Diagnostics.Debug.WriteLine($"PianoRollView::PianoRollKeyboard_KeyReleased value = {e.NoteValue}, velocity = {e.Velocity}");
        }
    }
}
