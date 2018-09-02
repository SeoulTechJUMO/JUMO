using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace JUMO.UI.Controls
{
    class BlockSelectionAdorner : Adorner
    {
        private static readonly Brush _rectangleBrush = new SolidColorBrush(Colors.DodgerBlue) { Opacity = 0.1 };
        private static readonly Pen _rectanglePen = new Pen(new SolidColorBrush(Colors.DodgerBlue) { Opacity = 0.5 }, 1.2);

        public static DependencyProperty Point1Property =
            DependencyProperty.Register(
                "Point1", typeof(Point), typeof(BlockSelectionAdorner),
                new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static DependencyProperty Point2Property =
            DependencyProperty.Register(
                "Point2", typeof(Point), typeof(BlockSelectionAdorner),
                new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public Point Point1
        {
            get => (Point)GetValue(Point1Property);
            set => SetValue(Point1Property, value);
        }

        public Point Point2
        {
            get => (Point)GetValue(Point2Property);
            set => SetValue(Point2Property, value);
        }

        public BlockSelectionAdorner(UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(_rectangleBrush, _rectanglePen, new Rect(Point1, Point2));
        }
    }
}
