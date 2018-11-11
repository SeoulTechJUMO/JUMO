using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JUMO.UI.Controls
{
    public class MusicalScrollViewer : ScrollViewer
    {
        #region Dependency Properties

        public static readonly DependencyProperty SmallGridBrushProperty =
            DependencyProperty.Register(
                "SmallGridBrush", typeof(Brush), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(Brushes.Silver)
            );

        public static readonly DependencyProperty BeatGridBrushProperty =
            DependencyProperty.Register(
                "BeatGridBrush", typeof(Brush), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(Brushes.Black)
            );

        public static readonly DependencyProperty BarGridBrushProperty =
            DependencyProperty.Register(
                "BarGridBrush", typeof(Brush), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(Brushes.Black)
            );

        public static readonly DependencyProperty BarGridThicknessProperty =
            DependencyProperty.Register(
                "BarGridThickness", typeof(double), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(2.0)
            );

        public static readonly DependencyProperty BarIndicatorProperty =
            DependencyProperty.Register(
                "BarIndicator", typeof(FrameworkElement), typeof(MusicalScrollViewer)
            );

        public static readonly DependencyProperty ShouldDrawHorizontalGridProperty =
            DependencyProperty.Register(
                "ShouldDrawHorizontalGrid", typeof(bool), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty GridStepProperty =
            DependencyProperty.Register(
                "GridStep", typeof(int), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(4)
            );

        public static readonly DependencyProperty GridHeightProperty =
            DependencyProperty.Register(
                "GridHeight", typeof(double), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(
                    20.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                )
            );

        #endregion

        #region Dependency Property Accessors

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

        public FrameworkElement BarIndicator
        {
            get => (FrameworkElement)GetValue(BarIndicatorProperty);
            set => SetValue(BarIndicatorProperty, value);
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

        #endregion

        static MusicalScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MusicalScrollViewer), new FrameworkPropertyMetadata(typeof(MusicalScrollViewer)));
        }
    }
}
