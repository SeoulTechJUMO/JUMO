using System.Windows.Controls.Primitives;
using System.Windows.Input;
using JUMO.UI.Controls;

namespace JUMO.UI.Views
{
    public partial class PatternPlacementView : SelectableView
    {
        public PatternPlacementView()
        {
            InitializeComponent();
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewMoveStarted(this);
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewMoveComplete(this);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewMoving(this, e.HorizontalChange, e.VerticalChange);
        }

        private void Thumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewLeftButtonDown(this);
        }

        private void Thumb_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewRightButtonDown(this);
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
    }
}
