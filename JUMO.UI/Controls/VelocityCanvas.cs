using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JUMO.UI.Data;
using JUMO.UI.Views;

namespace JUMO.UI.Controls
{
    class VelocityCanvas : InteractiveMusicalCanvas
    {
        private IEnumerable<NoteViewModel> _affectedNotes;
        private NoteViewModel _min, _max;

        #region MusicalCanvasBase Overrides

        protected override IVirtualElement CreateVirtualElementForItem(IMusicalItem item)
        {
            return new VirtualVelocityControl((NoteViewModel)item);
        }

        protected override double CalculateLogicalLength()
        {
            long length = Items.OfType<NoteViewModel>().Aggregate(0L, (acc, note) => Math.Max(acc, note.Start + note.Length));

            // 끝에 4분음표 8개 분량의 빈 공간을 둠
            return length + (TimeResolution << 3);
        }

        protected override Size CalculateSizeForElement(FrameworkElement element)
        {
            NoteViewModel note = (NoteViewModel)element.DataContext;

            double w = note.Length * WidthPerTick;
            double h = note.Velocity * ViewportHeight / 127.0;

            return new Size(w, h);
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            NoteViewModel note = (NoteViewModel)element.DataContext;

            double x = note.Start * WidthPerTick;
            double y = (127 - note.Velocity) * ViewportHeight / 127.0;
            double w = note.Length * WidthPerTick;
            double h = note.Velocity * ViewportHeight / 127.0;

            return new Rect(new Point(x, y), new Size(w, h));
        }

        #endregion

        protected override int MinVerticalValue => 0;
        protected override int MaxVerticalValue => 127;

        protected override int GetVerticalValue(IMusicalItem item) => ((NoteViewModel)item).Velocity;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) { }

        #region IMusicalViewCallback Members

        public override void MusicalViewResizeStarted(FrameworkElement view) => CalculateAffectedNotes(view);
        public override void MusicalViewResizeComplete(FrameworkElement view) => ViewEditComplete(view);

        public override void MusicalViewResizing(FrameworkElement view, double delta)
        {
            int deltaVelocity = -(int)Math.Round(delta * MaxVerticalValue / ActualHeight);

            if (delta > 0 && MinVerticalValue - _min.Velocity > deltaVelocity)
            {
                deltaVelocity = MinVerticalValue - _min.Velocity;
            }

            if (delta < 0 && MaxVerticalValue - _max.Velocity < deltaVelocity)
            {
                deltaVelocity = MaxVerticalValue - _max.Velocity;
            }

            foreach (NoteViewModel note in _affectedNotes)
            {
                AdjustVelocity(note, deltaVelocity);
            }
        }

        #endregion

        private void CalculateAffectedNotes(FrameworkElement view)
        {
            if (((NoteVelocityView)view).IsSelected)
            {
                _affectedNotes = SelectedItems.Cast<NoteViewModel>();
                (_min, _max) = _affectedNotes.MinMaxBy(note => note.Velocity);
            }
            else
            {
                _affectedNotes = new[] { (NoteViewModel)view.DataContext };
                _min = _max = (NoteViewModel)view.DataContext;
            }
        }

        private void ViewEditComplete(FrameworkElement view)
        {
            foreach (NoteViewModel note in _affectedNotes)
            {
                note.UpdateSource();
            }

            _affectedNotes = null;
        }

        private void AdjustVelocity(NoteViewModel note, int deltaVelocity)
        {
            int newVelocity = note.Velocity + deltaVelocity;
            note.Velocity = (byte)Math.Max(MinVerticalValue, Math.Min(newVelocity, MaxVerticalValue));
        }
    }
}
