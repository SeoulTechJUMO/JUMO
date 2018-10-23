using System.Windows;
using System.Windows.Media;

namespace JUMO.UI
{
    static class SkinHelper
    {
        public static readonly DependencyProperty IndicatorBrushProperty =
            DependencyProperty.RegisterAttached(
                "IndicatorBrush", typeof(Brush), typeof(SkinHelper),
                new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static Brush GetIndicatorBrush(UIElement target) => (Brush)target.GetValue(IndicatorBrushProperty);
        public static void SetIndicatorBrush(UIElement target, Brush value) => target.SetValue(IndicatorBrushProperty, value);
    }
}
