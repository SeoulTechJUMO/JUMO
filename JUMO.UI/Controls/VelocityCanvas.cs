using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using JUMO.UI.Data;
using JUMO.UI.Views;

namespace JUMO.UI.Controls
{
    class VelocityCanvas : MusicalCanvasBase, IMusicalViewCallback
    {
        private const byte MIN_VELOCITY = 0;
        private const byte MAX_VELOCITY = 127;

        private IEnumerable<Note> _affectedNotes;
        private Note _min, _max;

        #region MusicalCanvasBase Overrides

        protected override IVirtualElement CreateVirtualElementForItem(IMusicalItem item)
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

        public void MusicalViewResizeStarted(FrameworkElement view) => CalculateAffectedNotes(view);
        public void MusicalViewResizeComplete(FrameworkElement view) => ViewEditComplete(view);

        public void MusicalViewResizing(FrameworkElement view, double delta)
        {
            int deltaVelocity = -(int)Math.Round(delta * MAX_VELOCITY / ActualHeight);

            if (delta > 0 && MIN_VELOCITY - _min.Velocity > deltaVelocity)
            {
                deltaVelocity = MIN_VELOCITY - _min.Velocity;
            }

            if (delta < 0 && MAX_VELOCITY - _max.Velocity < deltaVelocity)
            {
                deltaVelocity = MAX_VELOCITY - _max.Velocity;
            }

            foreach (Note note in _affectedNotes)
            {
                AdjustVelocity(note, deltaVelocity);
            }
        }

        public void MusicalViewMoveStarted(FrameworkElement view) { }
        public void MusicalViewMoveComplete(FrameworkElement view) { }
        public void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY) { }

        public void MusicalViewLeftButtonDown(FrameworkElement view) { }
        public void MusicalViewRightButtonDown(FrameworkElement view) { }

        #endregion

        private void CalculateAffectedNotes(FrameworkElement view)
        {
            if (((NoteVelocityView)view).IsSelected)
            {
                _affectedNotes = SelectedItems.Cast<Note>();
                (_min, _max) = _affectedNotes.MinMaxBy(note => note.Velocity);
            }
            else
            {
                _affectedNotes = new[] { (Note)view.DataContext };
                _min = _max = (Note)view.DataContext;
            }
        }

        private void ViewEditComplete(FrameworkElement view)
        {
            _affectedNotes = null;
        }

        private void AdjustVelocity(Note note, int deltaVelocity)
        {
            int newVelocity = note.Velocity + deltaVelocity;
            note.Velocity = (byte)Math.Max(MIN_VELOCITY, Math.Min(newVelocity, MAX_VELOCITY));
        }
    }
}
