using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JUMO.UI.Data;
using JUMO.UI.Views;

namespace JUMO.UI.Controls
{
    class PianoRollCanvas : MusicalCanvasBase, IMusicalViewCallback
    {
        private const byte MIN_VALUE = 0;
        private const byte MAX_VALUE = 127;
        private const double FOLLOW_ACCEL = 0.0625;

        private IEnumerable<Note> _affectedNotes;
        private Note _minTick;
        private Note _minValue, _maxValue;

        private readonly BlockSelectionHelper _selectionHelper;

        public long _lastLength;

        #region Dependency Properties

        public static readonly DependencyProperty SnapToGridProperty =
            DependencyProperty.Register(
                "SnapToGrid", typeof(bool), typeof(PianoRollCanvas),
                new FrameworkPropertyMetadata(true)
            );

        public bool SnapToGrid
        {
            get => (bool)GetValue(SnapToGridProperty);
            set => SetValue(SnapToGridProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<AddNoteRequestedEventArgs> AddNoteRequested;
        public event EventHandler<DeleteNoteRequestedEventArgs> DeleteNoteRequested;

        #endregion

        public PianoRollCanvas() : base()
        {
            _selectionHelper = new BlockSelectionHelper(this);
            _lastLength = TimeResolution;
        }

        #region MusicalCanvasBase Overrides

        protected override IVirtualElement CreateVirtualElementForItem(object item)
        {
            return new VirtualNote((Note)item);
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

            return new Size(note.Length * WidthPerTick, 20);
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            Note note = (Note)element.DataContext;

            double x = note.Start * WidthPerTick;
            double y = (127 - note.Value) * 20;
            double w = note.Length * WidthPerTick;

            return new Rect(new Point(x, y), new Size(w, 20));
        }

        #endregion

        // LeftButtonDown - 노트 삽입
        // Ctrl+LeftButtonDown - 사각형 블록 선택 시작 (새 선택 영역)
        // Ctrl+Shift+LeftButtonDown - 사각형 블록 선택 시작 (선택 영역 추가)
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                Point pt = e.GetPosition(this);
                byte value = (byte)(127 - ((int)pt.Y / 20));
                long pos = PixelToTick(pt.X);
                long snap = SnapToGridInternal(pos);

                AddNoteRequested?.Invoke(this, new AddNoteRequestedEventArgs(new Note(value, 100, snap, _lastLength)));
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ClearSelection();
                _selectionHelper.StartBlockSelection(e.GetPosition(this));
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                _selectionHelper.StartBlockSelection(e.GetPosition(this));
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_selectionHelper.IsBlockSelecting)
            {
                Rect selectionRect = _selectionHelper.EndBlockSelection();

                long startTick = Math.Max(0L, PixelToTick(selectionRect.Left));
                long length = PixelToTick(selectionRect.Width);
                byte lowValue = (byte)Math.Max(0, Math.Min(127 - (int)selectionRect.Bottom / 20, 127));
                byte highValue = (byte)Math.Max(0, Math.Min(127 - (int)selectionRect.Top / 20, 127));

                var selectedNotes =
                    GetVirtualElementsInside(new Segment(startTick, length))
                    .Select(ve => (Note)((NoteView)ve.Visual).DataContext)
                    .Where(note => note.Value >= lowValue && note.Value <= highValue);

                SelectItems(new List<Note>(selectedNotes));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                _selectionHelper.UpdateBlockSelection(e.GetPosition(this));
                FollowMouse();
            }
        }

        private long PixelToTick(double xPos) => (long)(xPos / WidthPerTick);

        private long SnapToGridInternal(long pos)
        {
            if (!SnapToGrid)
            {
                return pos;
            }

            int ticksPerBeat = (TimeResolution << 2) / MusicalProps.GetDenominator(this);
            int ticksPerGrid = ticksPerBeat / GridStep;
            return (pos / ticksPerGrid) * ticksPerGrid;
        }

        #region IMusicalViewCallback Members

        public void MusicalViewResizeStarted(FrameworkElement view) => CalculateAffectedNotes(view);
        public void MusicalViewMoveStarted(FrameworkElement view) => CalculateAffectedNotes(view);
        public void MusicalViewResizeComplete(FrameworkElement view) => ViewEditComplete(view);
        public void MusicalViewMoveComplete(FrameworkElement view) => ViewEditComplete(view);

        public void MusicalViewResizing(FrameworkElement view, double delta)
        {
            foreach (Note note in _affectedNotes)
            {
                ResizeNote(note, delta);
            }

            FollowMouse();
        }

        public void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY)
        {
            long deltaStart = PixelToTick(deltaX);
            int deltaValue = -(int)deltaY / 20;

            if (deltaX < 0 && _minTick.Start < -deltaStart)
            {
                deltaStart = -_minTick.Start;
            }

            if (deltaY > 0 && MIN_VALUE - _minValue.Value > deltaValue)
            {
                deltaValue = MIN_VALUE - _minValue.Value;
            }

            if (deltaY < 0 && MAX_VALUE - _maxValue.Value < deltaValue)
            {
                deltaValue = MAX_VALUE - _maxValue.Value;
            }

            foreach (Note note in _affectedNotes)
            {
                MoveNote(note, deltaStart, deltaValue);
            }

            FollowMouse();
        }

        // LeftButtonDown - 노트 재생
        // Ctrl+LeftButtonDown - 노트 선택
        // Ctrl+Shift+LeftButtonDown - 현재 선택 영역에 노트 추가
        public void MusicalViewLeftButtonDown(FrameworkElement view)
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
        public void MusicalViewRightButtonDown(FrameworkElement view)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                DeleteNoteRequested?.Invoke(this, new DeleteNoteRequestedEventArgs((Note)view.DataContext));
            }
        }

        #endregion

        private void CalculateAffectedNotes(FrameworkElement view)
        {
            if (((NoteView)view).IsSelected)
            {
                _affectedNotes = SelectedItems.Cast<Note>();
                _minTick = _affectedNotes.MinBy(note => note.Start);
                (_minValue, _maxValue) = _affectedNotes.MinMaxBy(note => note.Value);
            }
            else
            {
                _affectedNotes = new[] { (Note)view.DataContext };
                _minTick = _minValue = _maxValue = (Note)view.DataContext;
            }
        }

        private void ViewEditComplete(FrameworkElement view)
        {
            _affectedNotes = null;
            _lastLength = ((Note)view.DataContext).Length;
        }

        private void ResizeNote(Note note, double delta)
        {
            long end = note.Start + note.Length;
            long newEnd = SnapToGridInternal(end + PixelToTick(delta));

            if (newEnd > note.Start)
            {
                note.Length = newEnd - note.Start;
            }
        }

        private void MoveNote(Note note, long deltaStart, int deltaValue)
        {
            long newStart = SnapToGridInternal(note.Start + deltaStart);
            int newValue = note.Value + deltaValue;

            note.Start = Math.Max(0, newStart);
            note.Value = (byte)Math.Max(MIN_VALUE, Math.Min(newValue, MAX_VALUE));
        }

        private void FollowMouse()
        {
            Point pos = Mouse.GetPosition(this) - new Vector(HorizontalOffset, VerticalOffset);

            if (pos.X > ViewportWidth)
            {
                SetHorizontalOffset(HorizontalOffset + (pos.X - ViewportWidth) * FOLLOW_ACCEL);
            }
            else if (pos.X < 0)
            {
                SetHorizontalOffset(HorizontalOffset + pos.X * FOLLOW_ACCEL);
            }

            if (pos.Y > ViewportHeight)
            {
                SetVerticalOffset(VerticalOffset + (pos.Y - ViewportHeight) * FOLLOW_ACCEL);
            }
            else if (pos.Y < 0)
            {
                SetVerticalOffset(VerticalOffset + pos.Y * FOLLOW_ACCEL);
            }
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
