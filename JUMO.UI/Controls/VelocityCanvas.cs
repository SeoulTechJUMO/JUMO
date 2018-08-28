using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JUMO.UI.Controls
{
    class VelocityCanvas : MusicalCanvasBase
    {
        #region Attached Properties

        public static readonly DependencyProperty VelocityProperty =
            DependencyProperty.RegisterAttached(
                "Velocity", typeof(int), typeof(VelocityCanvas),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty StartProperty =
            DependencyProperty.RegisterAttached(
                "Start", typeof(long), typeof(VelocityCanvas),
                new FrameworkPropertyMetadata(
                    0L,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.RegisterAttached(
                "Length", typeof(long), typeof(VelocityCanvas),
                new FrameworkPropertyMetadata(
                    0L,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        #endregion

        #region Attached Property Accessors

        public static int GetVelocity(UIElement target) => (int)target.GetValue(VelocityProperty);
        public static void SetVelocity(UIElement target, int value) => target.SetValue(VelocityProperty, value);
        public static long GetStart(UIElement target) => (long)target.GetValue(StartProperty);
        public static void SetStart(UIElement target, long value) => target.SetValue(StartProperty, value);
        public static long GetLength(UIElement target) => (long)target.GetValue(LengthProperty);
        public static void SetLength(UIElement target, long value) => target.SetValue(LengthProperty, value);

        #endregion

        protected override IVirtualElement CreateVirtualElementForItem(object item)
        {
            return new VirtualVelocityControl((Note)item);
        }

        protected override double CalculateLogicalLength()
        {
            long length = Items.OfType<Note>().Aggregate(0L, (acc, note) => Math.Max(acc, note.Start + note.Length));

            // 끝에 4분음표 8개 분량의 빈 공간을 둠
            return length + (TimeResolution << 3);
        }

        protected override Size CalculateSizeForElement(FrameworkElement element)
        {
            double w = GetLength(element) * WidthPerTick;
            double h = GetVelocity(element) * ViewportHeight / 127.0;

            return new Size(w, h);
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            double x = GetStart(element) * WidthPerTick;
            double y = (127 - GetVelocity(element)) * ViewportHeight / 127.0;
            double w = GetLength(element) * WidthPerTick;
            double h = GetVelocity(element) * ViewportHeight / 127.0;

            return new Rect(new Point(x, y), new Size(w, h));
        }
    }
}
