using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JUMO.UI.Views;

namespace JUMO.UI.Controls
{
    class PianoRollCanvas : MusicalCanvasBase, IMusicalViewCallback
    {
        #region Events

        public event EventHandler<AddNoteRequestedEventArgs> AddNoteRequested;
        public event EventHandler<DeleteNoteRequestedEventArgs> DeleteNoteRequested;

        #endregion

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
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
            }
        }

        public long PixelToTick(double xPos) => (long)(xPos * TimeResolution / (ZoomFactor << 2));

        public long SnapToGrid(long pos)
        {
            long ticksPerGrid = (long)(TimeResolution * (4.0 / GridUnit));
            return (pos / ticksPerGrid) * ticksPerGrid;
        }

        #region IMusicalViewCallback Members

        public void MusicalViewResizeStarted(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewMoveStarted(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewResizing(FrameworkElement view, double delta)
        {
            ResizeNote((Note)view.DataContext, delta);

            FollowMouse();
        }

        public void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY)
        {
            MoveNote((Note)view.DataContext, deltaX, deltaY);

            FollowMouse();
        }

        public void MusicalViewResizeComplete(FrameworkElement view) => ReIndexItem(view.DataContext);

        public void MusicalViewMoveComplete(FrameworkElement view) => ReIndexItem(view.DataContext);

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
                SetHorizontalOffset(HorizontalOffset + pos.X - ViewportWidth);
            }
            else if (pos.X < 0)
            {
                SetHorizontalOffset(HorizontalOffset + pos.X);
            }

            if (pos.Y > ViewportHeight)
            {
                SetVerticalOffset(VerticalOffset + pos.Y - ViewportHeight);
            }
            else if (pos.Y < 0)
            {
                SetVerticalOffset(VerticalOffset + pos.Y);
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
