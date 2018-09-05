using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace JUMO.UI.Controls
{
    class MusicalGrid : FrameworkElement
    {
        #region Dependency Properties

        private static readonly DependencyPropertyKey ContentBoundaryKey =
            DependencyProperty.RegisterReadOnly(
                "ContentBoundary", typeof(Rect), typeof(MusicalGrid),
                new FrameworkPropertyMetadata(
                    new Rect(),
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty ShouldDrawHorizontalGridProperty =
            DependencyProperty.Register(
                "ShouldDrawHorizontalGrid", typeof(bool), typeof(MusicalGrid),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty GridStepProperty =
            DependencyProperty.Register(
                "GridStep", typeof(int), typeof(MusicalGrid),
                new FrameworkPropertyMetadata(
                    4,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty GridHeightProperty =
            DependencyProperty.Register(
                "GridHeight", typeof(double), typeof(MusicalGrid),
                new FrameworkPropertyMetadata(
                    20.0,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty ContentBoundaryProperty = ContentBoundaryKey.DependencyProperty;

        #endregion

        #region Properties

        private int Numerator => MusicalProps.GetNumerator(this);
        private int Denominator => MusicalProps.GetDenominator(this);
        private int TimeResolution => MusicalProps.GetTimeResolution(this);
        private int ZoomFactor => MusicalProps.GetZoomFactor(this);

        public Rect ContentBoundary => (Rect)GetValue(ContentBoundaryProperty);

        public bool ShouldDrawHorizontalGrid
        {
            get => (bool)GetValue(ShouldDrawHorizontalGridProperty);
            set => SetValue(ShouldDrawHorizontalGridProperty, value);
        }

        public int GridStep
        {
            get => (int)GetValue(GridStepProperty);
            set => SetValue(GridStepProperty, value);
        }

        public double GridHeight
        {
            get => (double)GetValue(GridHeightProperty);
            set => SetValue(GridHeightProperty, value);
        }

        #endregion

        #region Drawing Resources

        private static readonly SolidColorBrush normalBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private static readonly SolidColorBrush fadedBrush = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));
        private static readonly Pen thickPen = new Pen(normalBrush, 2);
        private static readonly Pen normalPen = new Pen(normalBrush, 1);
        private static readonly Pen fadedPen = new Pen(fadedBrush, 1);

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            int gridCount = Numerator * GridStep;
            int ticksPerBeat = (TimeResolution << 2) / Denominator;

            double beatWidth = TickToPixel(ticksPerBeat);
            double barWidth = TickToPixel(ticksPerBeat * Numerator);
            double gridWidth = TickToPixel(ticksPerBeat / GridStep);

            SetValue(ContentBoundaryKey, new Rect(0, 0, barWidth, GridHeight));

            double cw = ContentBoundary.Width;
            double ch = ContentBoundary.Height;

            if (ShouldDrawHorizontalGrid)
            {
                dc.DrawLine(fadedPen, new Point(0, 0.5), new Point(cw, 0.5));
            }

            for (double xpos = 0; xpos <= cw; xpos += gridWidth)
            {
                dc.DrawLine(fadedPen, new Point(xpos + 0.5, 0), new Point(xpos + 0.5, ch));
            }

            for (double xpos = 0; xpos <= cw; xpos += beatWidth)
            {
                dc.DrawLine(normalPen, new Point(xpos + 0.5, 0), new Point(xpos + 0.5, ch));
            }

            dc.DrawLine(thickPen, new Point(0.5, 0), new Point(0.5, ch));
            dc.DrawLine(thickPen, new Point(cw + 0.5, 0), new Point(cw + 0.5, ch));
        }

        private double TickToPixel(long tick) => tick * (ZoomFactor << 2) / (double)TimeResolution;
    }
}
