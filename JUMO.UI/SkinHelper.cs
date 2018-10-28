using System.Windows;
using System.Windows.Media;

namespace JUMO.UI
{
    enum SegmentOption
    {
        None,
        Left,
        Center,
        Right
    }

    enum SkinVariant
    {
        Undefined,
        Light,
        Dark
    }

    static class SkinHelper
    {
        public static readonly DependencyProperty IndicatorBrushProperty =
            DependencyProperty.RegisterAttached(
                "IndicatorBrush", typeof(Brush), typeof(SkinHelper),
                new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyProperty.RegisterAttached(
                "BorderRadius", typeof(CornerRadius), typeof(SkinHelper),
                new FrameworkPropertyMetadata(new CornerRadius(2.0), FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static readonly DependencyProperty SegmentProperty =
            DependencyProperty.RegisterAttached(
                "Segment", typeof(SegmentOption), typeof(SkinHelper),
                new FrameworkPropertyMetadata(SegmentOption.None, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static readonly DependencyProperty VariantProperty =
            DependencyProperty.RegisterAttached(
                "Variant", typeof(SkinVariant), typeof(SkinHelper),
                new FrameworkPropertyMetadata(SkinVariant.Undefined, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static Brush GetIndicatorBrush(UIElement target) => (Brush)target.GetValue(IndicatorBrushProperty);
        public static void SetIndicatorBrush(UIElement target, Brush value) => target.SetValue(IndicatorBrushProperty, value);

        public static CornerRadius GetBorderRadius(UIElement target) => (CornerRadius)target.GetValue(BorderRadiusProperty);
        public static void SetBorderRadius(UIElement target, CornerRadius value) => target.SetValue(BorderRadiusProperty, value);

        public static SegmentOption GetSegment(UIElement target) => (SegmentOption)target.GetValue(SegmentProperty);
        public static void SetSegment(UIElement target, SegmentOption value) => target.SetValue(SegmentProperty, value);

        public static SkinVariant GetVariant(UIElement target) => (SkinVariant)target.GetValue(VariantProperty);
        public static void SetVariant(UIElement target, SkinVariant value) => target.SetValue(VariantProperty, value);
    }
}
