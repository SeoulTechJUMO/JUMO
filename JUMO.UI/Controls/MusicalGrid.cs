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

        public static readonly DependencyProperty SmallGridBrushProperty =
            DependencyProperty.Register(
                "SmallGridBrush", typeof(Brush), typeof(MusicalGrid),
                new FrameworkPropertyMetadata(
                    Brushes.Silver,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    GridBrushPropertyChangedCallback
                )
            );

        public static readonly DependencyProperty BeatGridBrushProperty =
            DependencyProperty.Register(
                "BeatGridBrush", typeof(Brush), typeof(MusicalGrid),
                new FrameworkPropertyMetadata(
                    Brushes.Black,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    GridBrushPropertyChangedCallback
                )
            );

        public static readonly DependencyProperty BarGridBrushProperty =
            DependencyProperty.Register(
                "BarGridBrush", typeof(Brush), typeof(MusicalGrid),
                new FrameworkPropertyMetadata(
                    Brushes.Black,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    GridBrushPropertyChangedCallback
                )
            );

        public static readonly DependencyProperty BarGridThicknessProperty =
            DependencyProperty.Register(
                "BarGridThickness", typeof(double), typeof(MusicalGrid),
                new FrameworkPropertyMetadata(
                    2.0,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    GridBrushPropertyChangedCallback
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
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender
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

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                "HorizontalOffset", typeof(double), typeof(MusicalGrid),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );


        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(
                "VerticalOffset", typeof(double), typeof(MusicalGrid),
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

        public Brush SmallGridBrush
        {
            get => (Brush)GetValue(SmallGridBrushProperty);
            set => SetValue(SmallGridBrushProperty, value);
        }

        public Brush BeatGridBrush
        {
            get => (Brush)GetValue(BeatGridBrushProperty);
            set => SetValue(BeatGridBrushProperty, value);
        }

        public Brush BarGridBrush
        {
            get => (Brush)GetValue(BarGridBrushProperty);
            set => SetValue(BarGridBrushProperty, value);
        }

        public double BarGridThickness
        {
            get => (double)GetValue(BarGridThicknessProperty);
            set => SetValue(BarGridThicknessProperty, value);
        }

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

        public double HorizontalOffset
        {
            get => (double)GetValue(HorizontalOffsetProperty);
            set => SetValue(HorizontalOffsetProperty, value);
        }

        public double VerticalOffset
        {
            get => (double)GetValue(VerticalOffsetProperty);
            set => SetValue(VerticalOffsetProperty, value);
        }

        #endregion

        private Pen _smallGridPen;
        private Pen _beatGridPen;
        private Pen _barGridPen;
        private double _beatWidth;
        private double _barWidth;
        private double _gridWidth;

        protected override void OnInitialized(EventArgs e)
        {
            UpdatePen();
            base.OnInitialized(e);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double tickWidth = ZoomFactor * 4 / TimeResolution;
            int ticksPerBeat = (TimeResolution << 2) / Denominator;

            _beatWidth = tickWidth * ticksPerBeat;
            _barWidth = tickWidth * ticksPerBeat * Numerator;
            _gridWidth = tickWidth * ticksPerBeat / GridStep;

            return base.MeasureOverride(availableSize);
        }

        protected override void OnRender(DrawingContext dc)
        {
            double rw = RenderSize.Width;
            double rh = RenderSize.Height;
            double hOffset = -HorizontalOffset;
            double vOffset = -VerticalOffset;

            if (ShouldDrawHorizontalGrid)
            {
                for (double ypos = vOffset % GridHeight; ypos <= rh; ypos += GridHeight)
                {
                    dc.DrawLine(_smallGridPen, new Point(0, ypos + 0.5), new Point(rw, ypos + 0.5));
                }
            }

            for (double xpos = hOffset % _gridWidth; xpos <= rw; xpos += _gridWidth)
            {
                dc.DrawLine(_smallGridPen, new Point(xpos + 0.5, 0), new Point(xpos + 0.5, rh));
            }

            for (double xpos = hOffset % _beatWidth; xpos <= rw; xpos += _beatWidth)
            {
                dc.DrawLine(_beatGridPen, new Point(xpos + 0.5, 0), new Point(xpos + 0.5, rh));
            }

            for (double xpos = hOffset % _barWidth; xpos <= rw; xpos += _barWidth)
            {
                dc.DrawLine(_barGridPen, new Point(xpos + 0.5, 0), new Point(xpos + 0.5, rh));
            }
        }

        private void UpdatePen()
        {
            _smallGridPen = new Pen(SmallGridBrush, 1);
            _beatGridPen = new Pen(BeatGridBrush, 1);
            _barGridPen = new Pen(BarGridBrush, BarGridThickness);
        }

        private static void GridBrushPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MusicalGrid)?.UpdatePen();
        }
    }
}
