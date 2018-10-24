using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JUMO.UI.Controls
{
    class Knob : RangeBase
    {
        private const double MinimumKnobRadius = 5.0;
        private const double Sqrt2 = 1.414213562373095;
        private const double AspectRatio = 1.171572875253809;

        private readonly VisualCollection _visuals;
        private readonly Ellipse _indicatorEllipse = new Ellipse() { SnapsToDevicePixels = true };
        private readonly Ellipse _faceEllipse = new Ellipse() { SnapsToDevicePixels = true };
        private readonly Ellipse _borderEllipse1 = new Ellipse() { SnapsToDevicePixels = true };
        private readonly Ellipse _borderEllipse2 = new Ellipse() { SnapsToDevicePixels = true };

        private double _totalRadius;
        private double _knobRadius;

        #region Dependency Properties

        public static readonly DependencyProperty KnobFaceProperty =
            DependencyProperty.Register(
                nameof(KnobFace), typeof(Brush), typeof(Knob),
                new FrameworkPropertyMetadata(SystemColors.ControlBrush, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static readonly DependencyProperty IndicatorProperty =
            DependencyProperty.Register(
                nameof(Indicator), typeof(Brush), typeof(Knob),
                new FrameworkPropertyMetadata(SystemColors.HighlightBrush, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public static readonly DependencyProperty TrackThicknessProperty =
            DependencyProperty.Register(
                nameof(TrackThickness), typeof(double), typeof(Knob),
                new FrameworkPropertyMetadata(
                    1.0,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    TrackMetricsPropertyChangedCallback
                )
            );

        public static readonly DependencyProperty TrackPaddingProperty =
            DependencyProperty.Register(
                nameof(TrackPadding), typeof(double), typeof(Knob),
                new FrameworkPropertyMetadata(
                    3.0,
                    FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsArrange,
                    TrackMetricsPropertyChangedCallback
                )
            );

        public static readonly DependencyProperty TrackLengthProperty =
            DependencyProperty.Register(
                nameof(TrackLength), typeof(double), typeof(Knob),
                new FrameworkPropertyMetadata(
                    3.0,
                    FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsArrange,
                    TrackMetricsPropertyChangedCallback
                )
            );

        public static readonly DependencyProperty KnobRadiusProperty =
            DependencyProperty.Register(
                nameof(KnobRadius), typeof(double), typeof(Knob),
                new FrameworkPropertyMetadata(
                    double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsArrange
                )
            );

        #endregion

        #region Properties

        public Brush KnobFace
        {
            get => (Brush)GetValue(KnobFaceProperty);
            set => SetValue(KnobFaceProperty, value);
        }

        public Brush Indicator
        {
            get => (Brush)GetValue(IndicatorProperty);
            set => SetValue(IndicatorProperty, value);
        }

        public double TrackThickness
        {
            get => (double)GetValue(TrackThicknessProperty);
            set => SetValue(TrackThicknessProperty, value);
        }

        public double TrackPadding
        {
            get => (double)GetValue(TrackPaddingProperty);
            set => SetValue(TrackPaddingProperty, value);
        }

        public double TrackLength
        {
            get => (double)GetValue(TrackLengthProperty);
            set => SetValue(TrackLengthProperty, value);
        }

        public double KnobRadius
        {
            get => (double)GetValue(KnobRadiusProperty);
            set => SetValue(KnobRadiusProperty, value);
        }

        #endregion

        public Knob()
        {
            _visuals = new VisualCollection(this)
            {
                _borderEllipse1,
                _borderEllipse2,
                _faceEllipse,
                _indicatorEllipse
            };

            Binding borderBinding = new Binding() { Path = new PropertyPath(BorderBrushProperty), Source = this };

            _indicatorEllipse.SetBinding(Shape.FillProperty, new Binding() { Path = new PropertyPath(IndicatorProperty), Source = this });
            _faceEllipse.SetBinding(Shape.FillProperty, new Binding() { Path = new PropertyPath(KnobFaceProperty), Source = this });
            _borderEllipse1.SetBinding(Shape.FillProperty, borderBinding);
            _borderEllipse2.SetBinding(Shape.FillProperty, borderBinding);
        }

        protected override int VisualChildrenCount => _visuals.Count;

        protected override Visual GetVisualChild(int index) => _visuals[index];

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            InvalidateArrange();

            base.OnValueChanged(oldValue, newValue);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double trackSize = TrackPadding + TrackLength;

            if (double.IsNaN(KnobRadius))
            {
                // Try to automatically calculate KnobRadius.

                double cWidth = constraint.Width;
                double cHeight = constraint.Height;

                if (double.IsInfinity(cWidth) && double.IsInfinity(cHeight))
                {
                    // There is no size constraint; just report the smallest size possible.

                    _knobRadius = MinimumKnobRadius;
                    _totalRadius = _knobRadius + trackSize;
                }
                else if (cWidth / cHeight >= AspectRatio)
                {
                    // Calculate KnobRadius using constraint.Height

                    double k1 = (2 - Sqrt2) * cHeight - trackSize;
                    double k2 = (cHeight - trackSize) / 2;

                    _knobRadius = trackSize / k1 > Sqrt2 - 1 ? k1 : k2;
                    _totalRadius = _knobRadius + trackSize;
                }
                else
                {
                    // Calculate KnobRadius using constraint.Width

                    _totalRadius = cWidth / 2;
                    _knobRadius = _totalRadius - trackSize;
                }
            }
            else
            {
                _knobRadius = KnobRadius;
                _totalRadius = _knobRadius + trackSize;
            }

            return new Size(2 * _totalRadius, _totalRadius + Math.Max(_totalRadius * Sqrt2 / 2, _knobRadius));
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            double centerX = arrangeBounds.Width / 2;
            double centerY = arrangeBounds.Height / 2;
            double diameter = 2 * _knobRadius;

            double valueAngle = (1.25 - 1.5 * (Value - Minimum) / (Maximum - Minimum)) * Math.PI;
            double indicatorX = (_knobRadius - 3) * Math.Cos(valueAngle) - 1 + centerX;
            double indicatorY = (_knobRadius - 3) * -Math.Sin(valueAngle) - 1 + centerY;

            double faceStartX = centerX - _knobRadius;
            double faceStartY = centerY - _knobRadius;

            _borderEllipse1.Arrange(new Rect(faceStartX - 1, faceStartY - 1, diameter + 2, diameter + 2));
            _borderEllipse2.Arrange(new Rect(faceStartX - 1, faceStartY, diameter + 2, diameter + 2));
            _faceEllipse.Arrange(new Rect(faceStartX, faceStartY, diameter, diameter));
            _indicatorEllipse.Arrange(new Rect(indicatorX, indicatorY, 2, 2));

            return arrangeBounds;
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(new Point(0, 0), RenderSize));

            double renderWidth = RenderSize.Width;
            double renderHeight = RenderSize.Height;
            double centerX = renderWidth / 2;
            double centerY = renderHeight / 2;

            Vector knobCenter = new Vector(renderWidth / 2, renderHeight / 2);
            double trackStartR = _knobRadius + TrackPadding;

            Pen trackPen = new Pen(Foreground, TrackThickness);

            if (TrackLength > 0)
            {
                double trackEndR = trackStartR + TrackLength;

                for (int i = -2; i <= 10; i++)
                {
                    double angle = i * Math.PI / 8;
                    double cosA = Math.Cos(angle);
                    double sinA = -Math.Sin(angle);
                    Point start = new Point(trackStartR * cosA, trackStartR * sinA) + knobCenter;
                    Point end = new Point(trackEndR * cosA, trackEndR * sinA) + knobCenter;

                    dc.DrawLine(trackPen, start, end);
                }
            }
        }

        private static void TrackMetricsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (double.IsNaN((double)e.NewValue))
            {
                throw new InvalidOperationException($"\"{e.Property.Name}\" cannot be automatically set.");
            }

            if ((double)e.NewValue < 0)
            {
                throw new InvalidOperationException($"\"{e.Property.Name}\" cannot be less than zero.");
            }
        }
    }
}
