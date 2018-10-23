using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JUMO.UI.Controls
{
    public partial class NumericUpDownEx : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(double), typeof(NumericUpDownEx),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    ValuePropertyChangedCallback,
                    CoerceValueCallback
                )
            );

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", typeof(double), typeof(NumericUpDownEx),
                new FrameworkPropertyMetadata(0.0, MinimumPropertyChangedCallback)
            );

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", typeof(double), typeof(NumericUpDownEx),
                new FrameworkPropertyMetadata(10.0, MaximumPropertyChangedCallback)
            );

        public static readonly DependencyProperty DeltaProperty =
            DependencyProperty.Register(
                "Delta", typeof(double), typeof(NumericUpDownEx),
                new FrameworkPropertyMetadata(1.0, DeltaPropertyChangedCallback)
            );

        #endregion

        #region Properties

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double Delta
        {
            get => (double)GetValue(DeltaProperty);
            set => SetValue(DeltaProperty, value);
        }

        #endregion

        public NumericUpDownEx()
        {
            InitializeComponent();
        }

        private void OnIncreaseButtonClick(object sender, RoutedEventArgs e) => Value += Delta;

        private void OnDecreaseButtonClick(object sender, RoutedEventArgs e) => Value -= Delta;

        #region Dependency Property Changed Callbacks

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private static void MinimumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoerceValueCallback(d, ((NumericUpDownEx)d).Value);
        }

        private static void MaximumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoerceValueCallback(d, ((NumericUpDownEx)d).Value);
        }

        private static void DeltaPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((double)e.NewValue <= 0)
            {
                throw new InvalidOperationException($"{nameof(Delta)} must be greater than zero.");
            }
        }

        private static object CoerceValueCallback(DependencyObject d, object baseValue)
        {
            NumericUpDownEx ctrl = (NumericUpDownEx)d;
            double clamp(double min, double x, double max) => Math.Max(min, Math.Min(x, max));

            switch (baseValue)
            {
                case double doubleValue:
                    return clamp(ctrl.Minimum, doubleValue, ctrl.Maximum);

                case string stringValue:
                    return double.TryParse(stringValue, out double parsedValue) ? clamp(ctrl.Minimum, parsedValue, ctrl.Maximum) : ctrl.Minimum;

                default:
                    return ctrl.Minimum;
            }
        }

        #endregion

        private void OnTextBoxMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Value += Delta;
            }
            else
            {
                Value -= Delta;
            }
        }
    }
}
