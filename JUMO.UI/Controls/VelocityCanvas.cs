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
            Note note = element.DataContext as Note;

            double w = (note?.Length ?? 0L) * WidthPerTick;
            double h = (note?.Velocity ?? 0) * ViewportHeight / 127.0;

            return new Size(w, h);
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            Note note = element.DataContext as Note;

            double x = (note?.Start ?? 0L) * WidthPerTick;
            double y = (127 - (note?.Velocity ?? 0)) * ViewportHeight / 127.0;
            double w = (note?.Length ?? 0L) * WidthPerTick;
            double h = (note?.Velocity ?? 0) * ViewportHeight / 127.0;

            return new Rect(new Point(x, y), new Size(w, h));
        }

        #region IMusicalViewCallback Members

        public void MusicalViewResizing(object musicalObject, double delta)
        {
            Note note = (Note)musicalObject;
            int newVelocity = (int)Math.Round(note.Velocity + (-delta) * (127.0 / ActualHeight));
            note.Velocity = (byte)Math.Max(0, Math.Min(newVelocity, 127));
        }

        public void MusicalViewResizeComplete(object musicalObject) { }
        public void MusicalViewMoving(object musicalObject, double deltaX, double deltaY) { }
        public void MusicalViewMoveComplete(object musicalObject) { }

        #endregion
    }
}
