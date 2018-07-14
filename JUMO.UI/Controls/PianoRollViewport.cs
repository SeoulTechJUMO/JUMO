using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JUMO.UI.Controls
{
    class PianoRollViewport : FrameworkElement
    {
        #region Dependency Properties

        public static readonly DependencyProperty NumeratorProperty =
            DependencyProperty.Register(
                "Numerator", typeof(int), typeof(PianoRollViewport),
                new PropertyMetadata(4, PropertyChangedCallback)
            );

        public static readonly DependencyProperty DenominatorProperty =
            DependencyProperty.Register(
                "Denominator", typeof(int), typeof(PianoRollViewport),
                new PropertyMetadata(4, PropertyChangedCallback)
            );

        public static readonly DependencyProperty TimeResolutionProperty =
            DependencyProperty.Register(
                "TimeResolution", typeof(int), typeof(PianoRollViewport),
                new PropertyMetadata(480, PropertyChangedCallback)
            );

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register(
                "ZoomFactor", typeof(int), typeof(PianoRollViewport),
                new PropertyMetadata(24, PropertyChangedCallback)
            );

        #endregion

        #region Properties

        public int Numerator
        {
            get => (int)GetValue(NumeratorProperty);
            set => SetValue(NumeratorProperty, value);
        }

        public int Denominator
        {
            get => (int)GetValue(DenominatorProperty);
            set => SetValue(DenominatorProperty, value);
        }

        public int TimeResolution
        {
            get => (int)GetValue(TimeResolutionProperty);
            set => SetValue(TimeResolutionProperty, value);
        }

        public int ZoomFactor
        {
            get => (int)GetValue(ZoomFactorProperty);
            set => SetValue(ZoomFactorProperty, value);
        }

        public int GridUnit => 16;

        private int PpqBase => (int)(TimeResolution / (Denominator / 4.0));

        private int PpqBar => PpqBase * Numerator;

        private int PpqGridUnit => (int)(TimeResolution / (GridUnit / 4.0));

        private int GridWidth => (int)(ZoomFactor * (16.0 / GridUnit));

        private double UnitsPerBase => PpqBase / PpqGridUnit;

        private double UnitsPerBar => PpqBar / PpqGridUnit;

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            System.Diagnostics.Debug.WriteLine($"TimeSignature = {Numerator}/{Denominator}, PPQN = {TimeResolution}, PPQ Base = {PpqBase}, PPQ Bar = {PpqBar}, PPQ GridUnit = {PpqGridUnit}");
            System.Diagnostics.Debug.WriteLine($"GridWidth = {GridWidth}, UnitsPerBase = {UnitsPerBase}, UnitsPerBar = {UnitsPerBar}");

            SolidColorBrush normalBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            SolidColorBrush fadedBrush = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));
            Pen thickPen = new Pen(normalBrush, 2);
            Pen normalPen = new Pen(normalBrush, 1);
            Pen fadedPen = new Pen(fadedBrush, 1);

            for (int i = 0; i < 100; i++)
            {
                Pen activePen = i % UnitsPerBar == 0 ? thickPen : (i % UnitsPerBase == 0 ? normalPen : fadedPen);
                double x = i * GridWidth + 0.5;
                dc.DrawLine(activePen, new Point(x, 0), new Point(x, ActualHeight));
            }
        }

        private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as PianoRollViewport).InvalidateVisual();
        }
    }
}
