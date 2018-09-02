using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JUMO.UI.Controls
{
    class VelocityCanvas : MusicalCanvasBase, IMusicalViewCallback
    {
        #region MusicalCanvasBase Overrides

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
            Note note = (Note)element.DataContext;

            double w = note.Length * WidthPerTick;
            double h = note.Velocity * ViewportHeight / 127.0;

            return new Size(w, h);
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            Note note = (Note)element.DataContext;

            double x = note.Start * WidthPerTick;
            double y = (127 - note.Velocity) * ViewportHeight / 127.0;
            double w = note.Length * WidthPerTick;
            double h = note.Velocity * ViewportHeight / 127.0;

            return new Rect(new Point(x, y), new Size(w, h));
        }

        #endregion

        #region IMusicalViewCallback Members

        public void MusicalViewResizeStarted(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewMoveStarted(FrameworkElement view) { }

        public void MusicalViewResizing(FrameworkElement view, double delta)
        {
            Note note = (Note)view.DataContext;
            int newVelocity = (int)Math.Round(note.Velocity + (-delta) * (127.0 / ActualHeight));
            note.Velocity = (byte)Math.Max(0, Math.Min(newVelocity, 127));
        }

        public void MusicalViewResizeComplete(FrameworkElement view) { }
        public void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY) { }
        public void MusicalViewMoveComplete(FrameworkElement view) { }
        public void MusicalViewLeftButtonDown(FrameworkElement view) { }
        public void MusicalViewRightButtonDown(FrameworkElement view) { }

        #endregion
    }
}
