using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace JUMO.UI.Controls
{
    class PlaybackBarAdorner : Adorner
    {
        private static readonly Pen _barPen = new Pen(Brushes.Lime, 1.5);
        private static readonly LinearGradientBrush _glowBrush = new LinearGradientBrush(Colors.Transparent, Color.FromArgb(128, 0, 255, 0), 0.0);

        private readonly FrameworkElement _adornedElement;

        public static readonly DependencyProperty PixelPositionProperty =
            DependencyProperty.Register(
                "PixelPosition", typeof(double), typeof(PlaybackBarAdorner),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(
                "VerticalOffset", typeof(double), typeof(PlaybackBarAdorner),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public double PixelPosition
        {
            get => (double)GetValue(PixelPositionProperty);
            set => SetValue(PixelPositionProperty, value);
        }

        public double VerticalOffset
        {
            get => (double)GetValue(VerticalOffsetProperty);
            set => SetValue(VerticalOffsetProperty, value);
        }

        public PlaybackBarAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _adornedElement = (FrameworkElement)AdornedElement;
            IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext dc)
        {
            double vOffset = VerticalOffset;
            double height = _adornedElement.RenderSize.Height;
            double xPos = PixelPosition;

            dc.DrawRectangle(_glowBrush, null, new Rect(xPos - 10, vOffset, 10, height));
            dc.DrawLine(_barPen, new Point(xPos, vOffset), new Point(xPos, vOffset + height));
        }
    }
}
