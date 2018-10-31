using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JUMO.UI.Controls
{
    class Knob : RangeBase
    {
        private const double MinimumKnobRadius = 5.0;

        private static readonly ControlTemplate _thumbTemplate;

        private readonly VisualCollection _visuals;
        private readonly Ellipse _indicatorEllipse = new Ellipse();
        private readonly Ellipse _faceEllipse = new Ellipse();
        private readonly Ellipse _borderEllipse1 = new Ellipse();
        private readonly Ellipse _borderEllipse2 = new Ellipse();

        private readonly Thumb _thumb = new Thumb()
        {
            SnapsToDevicePixels = true,
            Template = _thumbTemplate
        };

        private double _totalRadius;
        private double _knobRadius;
        private double _startRelativeDragPos;

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

        static Knob()
        {
            string thumbTemplateXaml =
                "<ControlTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" TargetType=\"Thumb\"><Border><Rectangle Fill=\"Transparent\" /></Border></ControlTemplate>";

            _thumbTemplate = (ControlTemplate)XamlReader.Parse(thumbTemplateXaml);

            ValueProperty.OverrideMetadata(
                typeof(Knob),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    ValuePropertyChangedCallback,
                    CoerceValueCallback,
                    false,
                    UpdateSourceTrigger.Explicit)
                );
        }

        public Knob()
        {
            _visuals = new VisualCollection(this)
            {
                _borderEllipse1,
                _borderEllipse2,
                _faceEllipse,
                _indicatorEllipse,
                _thumb
            };

            Binding borderBinding = new Binding() { Path = new PropertyPath(BorderBrushProperty), Source = this };

            _indicatorEllipse.SetBinding(Shape.FillProperty, new Binding() { Path = new PropertyPath(IndicatorProperty), Source = this });
            _faceEllipse.SetBinding(Shape.FillProperty, new Binding() { Path = new PropertyPath(KnobFaceProperty), Source = this });
            _borderEllipse1.SetBinding(Shape.FillProperty, borderBinding);
            _borderEllipse2.SetBinding(Shape.FillProperty, borderBinding);

            _thumb.DragStarted += OnThumbDragStarted;
            _thumb.DragCompleted += OnThumbDragCompleted;
            _thumb.DragDelta += OnThumbDragDelta;
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
                    _knobRadius = MinimumKnobRadius;
                    _totalRadius = _knobRadius + trackSize;
                }
                else if (cWidth > cHeight)
                {
                    _totalRadius = cHeight / 2;
                    _knobRadius = _totalRadius - trackSize;
                }
                else
                {
                    _totalRadius = cWidth / 2;
                    _knobRadius = _totalRadius - trackSize;
                }
            }
            else
            {
                _knobRadius = KnobRadius;
                _totalRadius = _knobRadius + trackSize;
            }

            return new Size(2 * _totalRadius, 2 * _totalRadius);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            double diameter = 2 * _knobRadius;

            double valueAngle = (1.25 - 1.5 * (Value - Minimum) / (Maximum - Minimum)) * Math.PI;
            double indicatorX = (_knobRadius - 3) * Math.Cos(valueAngle) - 1 + _totalRadius;
            double indicatorY = (_knobRadius - 3) * -Math.Sin(valueAngle) - 1 + _totalRadius;

            double faceStart = _totalRadius - _knobRadius;
            Rect knobFaceRect = new Rect(faceStart, faceStart, diameter, diameter);

            _borderEllipse1.Arrange(new Rect(faceStart - 1, faceStart - 1, diameter + 2, diameter + 2));
            _borderEllipse2.Arrange(new Rect(faceStart - 1, faceStart, diameter + 2, diameter + 2));
            _faceEllipse.Arrange(knobFaceRect);
            _indicatorEllipse.Arrange(new Rect(indicatorX, indicatorY, 2, 2));
            _thumb.Arrange(knobFaceRect);

            return new Size(2 * _totalRadius, 2 * _totalRadius);
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(new Point(0, 0), RenderSize));

            Vector ccenter = new Vector(RenderSize.Width / 2, RenderSize.Height / 2);

            if (TrackLength > 0)
            {
                double trackStartR = _knobRadius + TrackPadding;
                double trackEndR = trackStartR + TrackLength;
                Pen trackPen = new Pen(Foreground, TrackThickness);

                for (int i = -2; i <= 10; i++)
                {
                    double angle = i * Math.PI / 8;
                    double cosA = Math.Cos(angle);
                    double sinA = -Math.Sin(angle);
                    Point start = new Point(trackStartR * cosA, trackStartR * sinA) + ccenter;
                    Point end = new Point(trackEndR * cosA, trackEndR * sinA) + ccenter;

                    dc.DrawLine(trackPen, start, end);
                }
            }
        }

        private void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            _startRelativeDragPos = 150 * (Value - Minimum) / (Maximum - Minimum);
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
        }

        private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Value = (_startRelativeDragPos - e.VerticalChange) * (Maximum - Minimum) / 150 + Minimum;
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

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Knob)d)?.GetBindingExpression(ValueProperty)?.UpdateSource();
        }

        private static object CoerceValueCallback(DependencyObject d, object baseValue)
        {
            Knob ctrl = (Knob)d;

            return Math.Max(ctrl.Minimum, Math.Min((double)baseValue, ctrl.Maximum));
        }
    }
}
