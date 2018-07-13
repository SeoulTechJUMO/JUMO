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
        protected override void OnRender(DrawingContext dc)
        {
            System.Diagnostics.Debug.WriteLine($"Actual: {ActualWidth} x {ActualHeight}");
            System.Diagnostics.Debug.WriteLine($"RenderSize: {RenderSize}");
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(255, 0, 0)), null, new Rect(20, 20, 80, 60));
        }
    }
}
