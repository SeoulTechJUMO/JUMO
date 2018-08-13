using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        #endregion

        #region Dependency Property Accessors

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

        #endregion

        private double _widthPerTick;
        private int _ticksPerBar;

        protected override Size MeasureOverride(Size availableSize)
        {
            int ppqn = MusicalProps.GetTimeResolution(this);
            _widthPerTick = (MusicalProps.GetZoomFactor(this) << 2) / (double)ppqn;
            _ticksPerBar = (ppqn * MusicalProps.GetNumerator(this) * 4) / MusicalProps.GetDenominator(this);

            double w = double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width;
            double h = double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height;

            return new Size(w, h);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            double barWidth = _ticksPerBar * _widthPerTick;
            int nextBar = (int)(ScrollOffset / barWidth);
            double nextBarPos = nextBar * barWidth - ScrollOffset + 0.5;
            Pen barSeparatorPen = new Pen(Foreground, 2.0);

            dc.DrawRectangle(Background, null, new Rect(new Point(0, 0), RenderSize));

            while (nextBarPos <= RenderSize.Width)
            {
                dc.DrawLine(barSeparatorPen, new Point(nextBarPos, 0), new Point(nextBarPos, RenderSize.Height));
                dc.DrawText(
                    new FormattedText(
                        $"{nextBar + 1}",
                        CultureInfo.CurrentUICulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"),
                        12.0,
                        Foreground
                    ),
                    new Point(nextBarPos + 3, 0)
                );
                nextBar += 1;
                nextBarPos += barWidth;
            }
        }
    }
}
