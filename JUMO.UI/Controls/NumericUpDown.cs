using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace JUMO.UI.Controls
{
    public class NumericUpDown : Control
    {
        #region Dependency Properties

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(double), typeof(NumericUpDown),
                new PropertyMetadata(0.0, ValueChangedCallback, CoerceValue)
            );

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                "MinValue", typeof(double), typeof(NumericUpDown),
                new PropertyMetadata(0.0)
            );

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                "MaxValue", typeof(double), typeof(NumericUpDown),
                new PropertyMetadata(100.0)
            );

        public static readonly DependencyProperty DeltaProperty =
            DependencyProperty.Register(
                "Delta", typeof(double), typeof(NumericUpDown),
                new PropertyMetadata(1.0)
            );

        #endregion

        #region Routed Events

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                "ValueChanged",
                RoutingStrategy.Direct,
                typeof(ValueChangedEventHandler),
                typeof(NumericUpDown)
            );

        #endregion

        #region Properties

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public double Delta
        {
            get => (double)GetValue(DeltaProperty);
            set => SetValue(DeltaProperty, value);
        }

        #endregion

        #region Events

        public event ValueChangedEventHandler ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        #endregion

        #region Named Parts

        private RepeatButton upButtonElement;

        private RepeatButton downButtonElement;

        private RepeatButton UpButtonElement
        {
            get => upButtonElement;
            set
            {
                if (upButtonElement != null)
                {
                    upButtonElement.Click -= UpButtonElement_Click;
                }

                upButtonElement = value;

                if (upButtonElement != null)
                {
                    upButtonElement.Click += UpButtonElement_Click;
                }
            }
        }

        private RepeatButton DownButtonElement
        {
            get => downButtonElement;
            set
            {
                if (downButtonElement != null)
                {
                    downButtonElement.Click -= DownButtonElement_Click;
                }

                downButtonElement = value;

                if (downButtonElement != null)
                {
                    downButtonElement.Click += DownButtonElement_Click;
                }
            }
        }

        private void UpButtonElement_Click(object sender, RoutedEventArgs e) => Value += Delta;

        private void DownButtonElement_Click(object sender, RoutedEventArgs e) => Value -= Delta;

        #endregion

        public NumericUpDown()
        {
            DefaultStyleKey = typeof(NumericUpDown);
            IsTabStop = true;
        }

        protected virtual void OnValueChanged(ValueChangedEventArgs e) => RaiseEvent(e);

        #region Overrides

        public override void OnApplyTemplate()
        {
            UpButtonElement = GetTemplateChild("UpButton") as RepeatButton;
            DownButtonElement = GetTemplateChild("DownButton") as RepeatButton;
        }

        #endregion

        #region Dependency Property Callbacks

        private static void ValueChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown ctrl = obj as NumericUpDown;
            double newValue = (double)e.NewValue;

            ctrl.OnValueChanged(new ValueChangedEventArgs(ValueChangedEvent, newValue));
        }

        private static object CoerceValue(DependencyObject obj, object baseValue)
        {
            NumericUpDown ctrl = obj as NumericUpDown;
            double clamp(double val) => Math.Max(ctrl.MinValue, Math.Min(ctrl.MaxValue, val));

            switch (baseValue)
            {
                case double d:
                    return clamp(d);
                case string s:
                    double x;
                    return double.TryParse(s, out x) ? clamp(x) : 0.0;
                default:
                    return 0.0;
            }
        }

        #endregion
    }

    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

    public class ValueChangedEventArgs : RoutedEventArgs
    {
        public double Value { get; }

        public ValueChangedEventArgs(RoutedEvent id, double value)
        {
            Value = value;
            RoutedEvent = id;
        }
    }
}
