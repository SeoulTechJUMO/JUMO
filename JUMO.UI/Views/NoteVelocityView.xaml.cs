using JUMO.UI.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for NoteVelocityView.xaml
    /// </summary>
    public partial class NoteVelocityView : SelectableView
    {
        public NoteVelocityView()
        {
            InitializeComponent();
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizeStarted(this);
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizeComplete(this);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizing(this, e.VerticalChange);
        }
    }
}
