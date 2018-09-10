using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using JUMO.UI.Controls;

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for PatternPlacementView.xaml
    /// </summary>
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
    }
}
