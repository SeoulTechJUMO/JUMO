using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for NoteView.xaml
    /// </summary>
    public partial class NoteView : UserControl
    {
        private bool _isSelected = false;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                UpdateVisualStates();
            }
        }

        public NoteView()
        {
            InitializeComponent();
        }

        private void UpdateVisualStates()
        {
            if (IsSelected)
            {
                VisualStateManager.GoToState(this, "Selected", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unselected", false);
            }
        }

        private void ResizeHandle_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizeComplete(this);
        }

        private void ResizeHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizing(this, e.HorizontalChange);
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
