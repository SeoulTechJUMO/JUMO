using System.Windows;
using System.Windows.Media;

namespace JUMO.UI
{
    static class ToggleButtonIndicator
    {
        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.RegisterAttached(
                "Brush", typeof(Brush), typeof(ToggleButtonIndicator),
                new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static Brush GetBrush(UIElement target) => (Brush)target.GetValue(BrushProperty);
        public static void SetBrush(UIElement target, Brush value) => target.SetValue(BrushProperty, value);
    }
}
