using JUMO.UI.Controls;
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

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for PatternPlacementView.xaml
    /// </summary>
    public partial class PatternPlacementView : UserControl
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

        public PatternPlacementView()
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
