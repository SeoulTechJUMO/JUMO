using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JUMO.UI.Data;
using JUMO.UI.Views;

namespace JUMO.UI.Controls
{
    class PianoRollCanvas : InteractiveMusicalCanvas
    {
        private IEnumerable<NoteViewModel> _affectedNotes;
        private NoteViewModel _minTick;
        private NoteViewModel _minValue, _maxValue;

        public long _lastLength;

        #region Events

        public event EventHandler<AddNoteRequestedEventArgs> AddNoteRequested;
        public event EventHandler<DeleteNoteRequestedEventArgs> DeleteNoteRequested;

        #endregion

        public PianoRollCanvas() : base()
        {
            _lastLength = TimeResolution;
        }

        #region MusicalCanvasBase Overrides

        protected override IVirtualElement CreateVirtualElementForItem(IMusicalItem item)
        {
            return new VirtualNote((NoteViewModel)item);
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

            return new Size(note.Length * WidthPerTick, 20);
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            NoteViewModel note = (NoteViewModel)element.DataContext;

            double x = note.Start * WidthPerTick;
            double y = (127 - note.Value) * 20;
            double w = note.Length * WidthPerTick;

            return new Rect(new Point(x, y), new Size(w, 20));
        }

        #endregion

        #region InteractiveMusicalCanvas Overrides

        protected override int MinVerticalValue => 0;
        protected override int MaxVerticalValue => 127;

        protected override int GetVerticalValue(IMusicalItem item) => ((NoteViewModel)item).Value;
        protected override int FromVerticalPosition(double y) => 127 - (int)y / 20;
        protected override int FromVerticalDelta(double dy) => -(int)dy / 20;

        protected override void OnSurfaceClick(Point pt)
        {
            byte value = (byte)FromVerticalPosition(pt.Y);
            long pos = PixelToTick(pt.X);
            long snap = SnapToGridInternal(pos);

            AddNoteRequested?.Invoke(this, new AddNoteRequestedEventArgs(new Note(value, 100, snap, _lastLength)));
        }

        #endregion

        #region IMusicalViewCallback Members

        public override void MusicalViewResizeStarted(FrameworkElement view) => CalculateAffectedNotes(view);
        public override void MusicalViewMoveStarted(FrameworkElement view) => CalculateAffectedNotes(view);
        public override void MusicalViewResizeComplete(FrameworkElement view) => ViewEditComplete(view);
        public override void MusicalViewMoveComplete(FrameworkElement view) => ViewEditComplete(view);

        public override void MusicalViewResizing(FrameworkElement view, double delta)
        {
            foreach (NoteViewModel note in _affectedNotes)
            {
                ResizeNote(note, delta);
            }

            FollowMouse();
        }

        public override void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY)
        {
            long deltaStart = PixelToTick(deltaX);
            int deltaValue = FromVerticalDelta(deltaY);

            if (deltaX < 0 && _minTick.Start < -deltaStart)
            {
                deltaStart = -_minTick.Start;
            }

            if (deltaY > 0 && MinVerticalValue - _minValue.Value > deltaValue)
            {
                deltaValue = MinVerticalValue - _minValue.Value;
            }

            if (deltaY < 0 && MaxVerticalValue - _maxValue.Value < deltaValue)
            {
                deltaValue = MaxVerticalValue - _maxValue.Value;
            }

            foreach (NoteViewModel note in _affectedNotes)
            {
                MoveNote(note, deltaStart, deltaValue);
            }

            FollowMouse();
        }

        // LeftButtonDown - 노트 재생
        // Ctrl+LeftButtonDown - 노트 선택
        // Ctrl+Shift+LeftButtonDown - 현재 선택 영역에 노트 추가
        public override void MusicalViewLeftButtonDown(FrameworkElement view)
        {
            NoteView noteView = (NoteView)view;

            if (Keyboard.Modifiers == ModifierKeys.None)
            {
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ClearSelection();
                SelectItem(noteView.DataContext);
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                if (noteView.IsSelected)
                {
                    DeselectItem(noteView.DataContext);
                }
                else
                {
                    SelectItem(noteView.DataContext);
                }
            }
        }

        // RightButtonDown - 노트 제거
        public override void MusicalViewRightButtonDown(FrameworkElement view)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                Note note = ((NoteViewModel)view.DataContext).Source;

                DeleteNoteRequested?.Invoke(this, new DeleteNoteRequestedEventArgs(note));
            }
        }

        #endregion

        private void CalculateAffectedNotes(FrameworkElement view)
        {
            if (((NoteView)view).IsSelected)
            {
                _affectedNotes = SelectedItems.Cast<NoteViewModel>();
                _minTick = _affectedNotes.MinBy(note => note.Start);
                (_minValue, _maxValue) = _affectedNotes.MinMaxBy(note => note.Value);
            }
            else
            {
                _affectedNotes = new[] { (NoteViewModel)view.DataContext };
                _minTick = _minValue = _maxValue = (NoteViewModel)view.DataContext;
            }
        }

        private void ViewEditComplete(FrameworkElement view)
        {
            foreach (NoteViewModel note in _affectedNotes)
            {
                note.UpdateSource();
            }

            _affectedNotes = null;
            _lastLength = ((NoteViewModel)view.DataContext).Length;
        }

        private void ResizeNote(NoteViewModel note, double delta)
        {
            long end = note.Start + note.Length;
            long newEnd = SnapToGridInternal(end + PixelToTick(delta));

            if (newEnd > note.Start)
            {
                note.Length = newEnd - note.Start;
            }
        }

        private void MoveNote(NoteViewModel note, long deltaStart, int deltaValue)
        {
            long newStart = SnapToGridInternal(note.Start + deltaStart);
            int newValue = note.Value + deltaValue;

            note.Start = Math.Max(0, newStart);
            note.Value = (byte)Math.Max(MinVerticalValue, Math.Min(newValue, MaxVerticalValue));
        }
    }

    class AddNoteRequestedEventArgs : EventArgs
    {
        public Note Note { get; }

        public AddNoteRequestedEventArgs(Note note)
        {
            Note = note;
        }
    }

    class DeleteNoteRequestedEventArgs : EventArgs
    {
        public IEnumerable<Note> NotesToDelete { get; }

        public DeleteNoteRequestedEventArgs(Note noteToDelete)
        {
            NotesToDelete = new Note[] { noteToDelete };
        }

        public DeleteNoteRequestedEventArgs(IEnumerable<Note> notesToDelete)
        {
            NotesToDelete = notesToDelete;
        }
    }
}
