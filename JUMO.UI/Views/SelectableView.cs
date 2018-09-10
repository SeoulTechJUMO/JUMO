using System.Windows;
using System.Windows.Controls;

namespace JUMO.UI.Views
{
    public abstract class SelectableView : UserControl
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

        protected virtual void UpdateVisualStates()
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
    }
}
