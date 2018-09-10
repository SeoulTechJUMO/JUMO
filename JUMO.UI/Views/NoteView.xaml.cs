using System.Windows.Controls.Primitives;
using System.Windows.Input;
using JUMO.UI.Controls;

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for NoteView.xaml
    /// </summary>
    public partial class NoteView : SelectableView
    {
        public NoteView()
        {
            InitializeComponent();
        }

        private void ResizeHandle_DragStarted(object sender, DragStartedEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizeStarted(this);
        }

        private void ResizeHandle_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizeComplete(this);
        }

        private void ResizeHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizing(this, e.HorizontalChange);
        }

        private void NoteButton_DragStarted(object sender, DragStartedEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewMoveStarted(this);
        }

        private void NoteButton_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewMoveComplete(this);
        }

        private void NoteButton_DragDelta(object sender, DragDeltaEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewMoving(this, e.HorizontalChange, e.VerticalChange);
        }

        private void NoteButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewLeftButtonDown(this);
        }

        private void NoteButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewRightButtonDown(this);
        }
    }
}
