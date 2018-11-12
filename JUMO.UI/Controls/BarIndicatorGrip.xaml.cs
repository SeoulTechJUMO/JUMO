using System.Windows;
using System.Windows.Controls;

namespace JUMO.UI.Controls
{
    public partial class BarIndicatorGrip : UserControl
    {
        public BarIndicatorGrip()
        {
            InitializeComponent();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(new Size(16, 16));
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return base.ArrangeOverride(new Size(16, 16));
        }
    }
}
