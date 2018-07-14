using System.Windows;

namespace JUMO.UI.Controls
{
    abstract class MusicalElement : FrameworkElement
    {
        #region Dependency Properties

        public static readonly DependencyProperty NumeratorProperty =
            DependencyProperty.Register(
                "Numerator", typeof(int), typeof(MusicalElement),
                new PropertyMetadata(4)
            );

        public static readonly DependencyProperty DenominatorProperty =
            DependencyProperty.Register(
                "Denominator", typeof(int), typeof(MusicalElement),
                new PropertyMetadata(4)
            );

        public static readonly DependencyProperty TimeResolutionProperty =
            DependencyProperty.Register(
                "TimeResolution", typeof(int), typeof(MusicalElement),
                new PropertyMetadata(480)
            );

        #endregion

        #region Dependency Property Accessors

        public int Numerator
        {
            get => (int)GetValue(NumeratorProperty);
            set => SetValue(NumeratorProperty, value);
        }

        public int Denominator
        {
            get => (int)GetValue(DenominatorProperty);
            set => SetValue(DenominatorProperty, value);
        }

        public int TimeResolution
        {
            get => (int)GetValue(TimeResolutionProperty);
            set => SetValue(TimeResolutionProperty, value);
        }

        #endregion

        #region Properties

        protected int TicksPerBeat => (int)(TimeResolution / (Denominator / 4.0));

        protected int TicksPerBar => TicksPerBeat * Numerator;

        #endregion
    }
}
