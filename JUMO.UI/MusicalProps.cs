using System.Windows;

namespace JUMO.UI
{
    static class MusicalProps
    {
        public static readonly DependencyProperty NumeratorProperty =
            DependencyProperty.RegisterAttached(
                "Numerator", typeof(int), typeof(MusicalProps),
                new FrameworkPropertyMetadata(
                    4,
                    FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.Inherits
                )
            );

        public static readonly DependencyProperty DenominatorProperty =
            DependencyProperty.RegisterAttached(
                "Denominator", typeof(int), typeof(MusicalProps),
                new FrameworkPropertyMetadata(
                    4,
                    FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.Inherits
                )
            );

        public static readonly DependencyProperty TimeResolutionProperty =
            DependencyProperty.RegisterAttached(
                "TimeResolution", typeof(int), typeof(MusicalProps),
                new FrameworkPropertyMetadata(
                    480,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.Inherits
                )
            );

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.RegisterAttached(
                "ZoomFactor", typeof(double), typeof(MusicalProps),
                new FrameworkPropertyMetadata(
                    24.0,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.Inherits
                )
            );

        public static int GetNumerator(UIElement target) => (int)target.GetValue(NumeratorProperty);
        public static void SetNumerator(UIElement target, int value) => target.SetValue(NumeratorProperty, value);

        public static int GetDenominator(UIElement target) => (int)target.GetValue(DenominatorProperty);
        public static void SetDenominator(UIElement target, int value) => target.SetValue(DenominatorProperty, value);

        public static int GetTimeResolution(UIElement target) => (int)target.GetValue(TimeResolutionProperty);
        public static void SetTimeResolution(UIElement target, int value) => target.SetValue(TimeResolutionProperty, value);

        public static double GetZoomFactor(UIElement target) => (double)target.GetValue(ZoomFactorProperty);
        public static void SetZoomFactor(UIElement target, double value) => target.SetValue(ZoomFactorProperty, value);
    }
}
