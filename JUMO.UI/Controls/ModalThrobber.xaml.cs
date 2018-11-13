using System.Windows;
using System.Windows.Controls;

namespace JUMO.UI.Controls
{
    public partial class ModalThrobber : ContentControl
    {
        public static DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                nameof(IsActive), typeof(bool), typeof(ModalThrobber), new FrameworkPropertyMetadata(false)
            );

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public ModalThrobber()
        {
            InitializeComponent();
        }
    }
}
