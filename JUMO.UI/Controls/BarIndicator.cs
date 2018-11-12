using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace JUMO.UI.Controls
{
    class BarIndicator : FrameworkElement
    {
        #region Dependency Properties

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                "Background", typeof(Brush), typeof(BarIndicator),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                "Foreground", typeof(Brush), typeof(BarIndicator),
                new FrameworkPropertyMetadata(
                    Brushes.Black,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty ScrollOffsetProperty =
            DependencyProperty.Register(
                "ScrollOffset", typeof(double), typeof(BarIndicator),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register(
                "CurrentPosition", typeof(int), typeof(BarIndicator),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static readonly DependencyProperty ShouldDrawCurrentPositionProperty =
            DependencyProperty.Register(
                "ShouldDrawCurrentPosition", typeof(bool), typeof(BarIndicator),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        #endregion

        #region Properties

        private int Numerator => MusicalProps.GetNumerator(this);
        private int Denominator => MusicalProps.GetDenominator(this);
        private int TimeResolution => MusicalProps.GetTimeResolution(this);
        private double ZoomFactor => MusicalProps.GetZoomFactor(this);

        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public double ScrollOffset
        {
            get => (double)GetValue(ScrollOffsetProperty);
            set => SetValue(ScrollOffsetProperty, value);
        }

        public int CurrentPosition
        {
            get => (int)GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);
        }

        public bool ShouldDrawCurrentPosition
        {
            get => (bool)GetValue(ShouldDrawCurrentPositionProperty);
            set => SetValue(ShouldDrawCurrentPositionProperty, value);
        }

        #endregion

        private static readonly Typeface _typeface = new Typeface("Segoe UI");

        private double _tickWidth;
        private double _barWidth;

        protected override Size MeasureOverride(Size availableSize)
        {
            _tickWidth = ZoomFactor * 4 / TimeResolution;
            int ticksPerBar = (TimeResolution * Numerator * 4) / Denominator;

            _barWidth = ticksPerBar * _tickWidth;

            return base.MeasureOverride(availableSize);
        }

        protected override void OnRender(DrawingContext dc)
        {
            int nextBar = (int)(ScrollOffset / _barWidth);
            double nextBarPos = nextBar * _barWidth - ScrollOffset + 0.5;

            dc.DrawRectangle(Background, null, new Rect(new Point(0, 0), RenderSize));

            while (nextBarPos <= RenderSize.Width)
            {
                FormattedText barNumberText = new FormattedText(
                    $"{nextBar + 1}",
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    _typeface,
                    10.0,
                    Foreground
                );

                dc.DrawText(barNumberText, new Point(nextBarPos + 2, RenderSize.Height - barNumberText.Height));
                nextBar += 1;
                nextBarPos += _barWidth;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            CaptureMouse();
            SetPosition(e.GetPosition(this));
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                SetPosition(e.GetPosition(this));
            }
        }

        private void SetPosition(Point pt)
        {
            if (ShouldDrawCurrentPosition)
            {
                CurrentPosition = (int)((pt.X + ScrollOffset) / _tickWidth);
            }
        }
    }
}
