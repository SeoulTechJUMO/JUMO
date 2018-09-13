using System.Windows;

namespace JUMO.UI.Controls
{
    abstract class InteractiveMusicalCanvas : MusicalCanvasBase
    {
        #region Dependency Properties

        public static readonly DependencyProperty GridStepProperty =
            DependencyProperty.Register(
                "GridStep", typeof(int), typeof(InteractiveMusicalCanvas),
                new FrameworkPropertyMetadata(4)
            );

        #endregion

        #region Properties

        public int GridStep
        {
            get => (int)GetValue(GridStepProperty);
            set => SetValue(GridStepProperty, value);
        }

        #endregion
    }
}
