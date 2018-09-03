using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using JUMO.UI.Data;
using JUMO.UI.Views;

namespace JUMO.UI.Controls
{
    class PianoRollCanvas : MusicalCanvasBase, IMusicalViewCallback
    {
        private const double MIN_VALUE = 0;
        private const double MAX_VALUE = 127;
        private const double FOLLOW_ACCEL = 0.0625;

        private IEnumerable<Note> _affectedNotes;
        private Note _minTick;
        private Note _minValue, _maxValue;

        private readonly BlockSelectionAdorner _selectionAdorner;
        private bool _isSelecting = false;
        private Rect _selectionRect;

        #region Events

        public event EventHandler<AddNoteRequestedEventArgs> AddNoteRequested;
        public event EventHandler<DeleteNoteRequestedEventArgs> DeleteNoteRequested;

        #endregion

        public PianoRollCanvas() : base()
        {
            _selectionAdorner = new BlockSelectionAdorner(this);
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
                long snap = SnapToGrid(pos);

                AddNoteRequested?.Invoke(this, new AddNoteRequestedEventArgs(pos, snap, value));
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ClearSelection();
                StartBlockSelection(e.GetPosition(this));
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                StartBlockSelection(e.GetPosition(this));
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isSelecting)
            {
                EndBlockSelection();

                long startTick = Math.Max(0L, PixelToTick(_selectionRect.Left));
                long length = PixelToTick(_selectionRect.Width);
                byte lowValue = (byte)Math.Max(0, Math.Min(127 - (int)_selectionRect.Bottom / 20, 127));
                byte highValue = (byte)Math.Max(0, Math.Min(127 - (int)_selectionRect.Top / 20, 127));

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
                _selectionAdorner.Point2 = e.GetPosition(this);

                FollowMouse();
            }
        }

        private void StartBlockSelection(Point point1)
        {
            Mouse.Capture(this, CaptureMode.Element);
            AdornerLayer.GetAdornerLayer(this)?.Add(_selectionAdorner);

            _selectionAdorner.Point1 = _selectionAdorner.Point2 = point1;
            _isSelecting = true;
        }

        private void EndBlockSelection()
        {
            _isSelecting = false;
            _selectionRect = new Rect(_selectionAdorner.Point1, _selectionAdorner.Point2);

            AdornerLayer.GetAdornerLayer(this)?.Remove(_selectionAdorner);
            Mouse.Capture(null);
        }

        private long PixelToTick(double xPos) => (long)(xPos * TimeResolution / (ZoomFactor << 2));

        private long SnapToGrid(long pos)
        {
            long ticksPerGrid = (long)(TimeResolution * (4.0 / GridUnit));
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
            // TODO: BUG
            if (_minTick.Start <= 0 && deltaX <= 0)
            {
                deltaX = 0;
            }

            // TODO: BUG
            if ((_minValue.Value <= MIN_VALUE && deltaY >= 0)
                || (_maxValue.Value >= MAX_VALUE && deltaY <= 0))
            {
                deltaY = 0;
            }

            foreach (Note note in _affectedNotes)
            {
                MoveNote(note, deltaX, deltaY);
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
        }

        private void ResizeNote(Note note, double delta)
        {
            long end = note.Start + note.Length;
            long newEnd = SnapToGrid(end + PixelToTick(delta));

            if (newEnd > note.Start)
            {
                note.Length = newEnd - note.Start;
            }
        }

        private void MoveNote(Note note, double deltaX, double deltaY)
        {
            long newStart = SnapToGrid(note.Start + PixelToTick(deltaX));
            int newValue = note.Value - (int)deltaY / 20;

            note.Start = Math.Max(0, newStart);
            note.Value = (byte)Math.Max(0, Math.Min(newValue, 127));
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
        public long Position { get; }
        public long SnappedPosition { get; }
        public byte Value { get; }

        public AddNoteRequestedEventArgs(long position, long snappedPosition, byte value)
        {
            Position = position;
            SnappedPosition = snappedPosition;
            Value = value;
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
