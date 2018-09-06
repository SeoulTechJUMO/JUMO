﻿using System;
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

        #endregion

        private double _barWidth;

        protected override Size MeasureOverride(Size availableSize)
        {
            double tickWidth = ZoomFactor * 4 / TimeResolution;
            int ticksPerBar = (TimeResolution * Numerator * 4) / Denominator;

            _barWidth = ticksPerBar * tickWidth;

            return base.MeasureOverride(availableSize);
        }

        protected override void OnRender(DrawingContext dc)
        {
            int nextBar = (int)(ScrollOffset / _barWidth);
            double nextBarPos = nextBar * _barWidth - ScrollOffset + 0.5;
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
                nextBarPos += _barWidth;
            }
        }
    }
}
