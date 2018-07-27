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
    class MusicalGrid : MusicalElement
    {
        static MusicalGrid()
        {
            NumeratorProperty.OverrideMetadata(
                typeof(MusicalGrid),
                new PropertyMetadata(4, PropertyChangedCallback)
            );

            DenominatorProperty.OverrideMetadata(
                typeof(MusicalGrid),
                new PropertyMetadata(4, PropertyChangedCallback)
            );

            TimeResolutionProperty.OverrideMetadata(
                typeof(MusicalGrid),
                new PropertyMetadata(480, PropertyChangedCallback)
            );
        }

        public MusicalGrid()
        {
            ResizeContentBoundary();
        }

        #region Dependency Properties

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register(
                "ZoomFactor", typeof(int), typeof(MusicalGrid),
                new PropertyMetadata(24, PropertyChangedCallback)
            );

        private static readonly DependencyPropertyKey ContentBoundaryKey =
            DependencyProperty.RegisterReadOnly(
                "ContentBoundary", typeof(Rect), typeof(MusicalGrid),
                new PropertyMetadata(default(Rect))
            );

        public static readonly DependencyProperty GridUnitProperty =
            DependencyProperty.Register(
                "GridUnit", typeof(int), typeof(MusicalGrid),
                new PropertyMetadata(16, PropertyChangedCallback)
            );

        public static readonly DependencyProperty ShouldDrawHorizontalGridProperty =
            DependencyProperty.Register(
                "ShouldDrawHorizontalGrid", typeof(bool), typeof(MusicalGrid),
                new PropertyMetadata(true, PropertyChangedCallback)
            );

        public static readonly DependencyProperty GridHeightProperty =
            DependencyProperty.Register(
                "GridHeight", typeof(double), typeof(MusicalGrid),
                new PropertyMetadata(20.0, PropertyChangedCallback)
            );

        public static readonly DependencyProperty ContentBoundaryProperty = ContentBoundaryKey.DependencyProperty;

        #endregion

        #region Properties

        public int ZoomFactor
        {
            get => (int)GetValue(ZoomFactorProperty);
            set => SetValue(ZoomFactorProperty, value);
        }

        public Rect ContentBoundary => (Rect)GetValue(ContentBoundaryProperty);

        public int GridUnit
        {
            get => (int)GetValue(GridUnitProperty);
            set => SetValue(GridUnitProperty, value);
        }

        public bool ShouldDrawHorizontalGrid
        {
            get => (bool)GetValue(ShouldDrawHorizontalGridProperty);
            set => SetValue(ShouldDrawHorizontalGridProperty, value);
        }

        public double GridHeight
        {
            get => (double)GetValue(GridHeightProperty);
            set => SetValue(GridHeightProperty, value);
        }

        private int TicksPerGridUnit => (int)(TimeResolution / (GridUnit / 4.0));

        private double GridWidth => ZoomFactor * (16.0 / GridUnit);

        private double BeatWidth => ZoomFactor * (16.0 / Denominator);

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
            System.Diagnostics.Debug.WriteLine($"TimeSignature = {Numerator}/{Denominator}, PPQN = {TimeResolution}, Ticks Per GridUnit = {TicksPerGridUnit}, GridWidth = {GridWidth}, Content Boundary = {ContentBoundary}");

            double cw = ContentBoundary.Width;
            double ch = ContentBoundary.Height;

            if (ShouldDrawHorizontalGrid)
            {
                dc.DrawLine(fadedPen, new Point(0, 0.5), new Point(cw, 0.5));
            }

            for (double xpos = 0; xpos <= cw; xpos += GridWidth)
            {
                dc.DrawLine(fadedPen, new Point(xpos + 0.5, 0), new Point(xpos + 0.5, ch));
            }

            for (double xpos = 0; xpos <= cw; xpos += BeatWidth)
            {
                dc.DrawLine(normalPen, new Point(xpos + 0.5, 0), new Point(xpos + 0.5, ch));
            }

            dc.DrawLine(thickPen, new Point(0.5, 0), new Point(0.5, ch));
            dc.DrawLine(thickPen, new Point(cw + 0.5, 0), new Point(cw + 0.5, ch));
        }

        private void ResizeContentBoundary() => SetValue(ContentBoundaryKey, new Rect(0, 0, BeatWidth * Numerator, GridHeight));

        private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MusicalGrid ctrl = obj as MusicalGrid;

            ctrl.ResizeContentBoundary();
            ctrl.InvalidateVisual();
        }
    }
}
