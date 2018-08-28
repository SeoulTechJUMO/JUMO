using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JUMO.UI.Controls
{
    class PianoRollCanvas : MusicalCanvasBase, IMusicalViewCallback
    {
        #region Routed Events

        public static readonly RoutedEvent InteractionEvent =
            EventManager.RegisterRoutedEvent(
                "Interaction",
                RoutingStrategy.Direct,
                typeof(PianoRollInteractionEventHandler),
                typeof(PianoRollCanvas)
            );

        public event PianoRollInteractionEventHandler Interaction
        {
            add => AddHandler(InteractionEvent, value);
            remove => RemoveHandler(InteractionEvent, value);
        }

        #endregion

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
            Note note = element.DataContext as Note;

            return new Size((note?.Length ?? 0L) * WidthPerTick, 20);
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            Note note = element.DataContext as Note;

            double x = (note?.Start ?? 0L) * WidthPerTick;
            double y = (127 - (note?.Value ?? 0)) * 20;
            double w = (note?.Length ?? 0L) * WidthPerTick;

            return new Rect(new Point(x, y), new Size(w, 20));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(this);
            byte value = (byte)(127 - ((int)pt.Y / 20));
            long pos = PixelToTick(pt.X);
            long snap = SnapToGrid(pos);

            RaiseEvent(new PianoRollInteractionEventArgs(InteractionEvent, pos, snap, value, e));
            e.Handled = true;
        }

        public long PixelToTick(double xPos) => (long)(xPos * TimeResolution / (ZoomFactor << 2));

        public long SnapToGrid(long pos)
        {
            long ticksPerGrid = (long)(TimeResolution * (4.0 / GridUnit));
            return (pos / ticksPerGrid) * ticksPerGrid;
        }

        #region IMusicalViewCallback Members

        public void MusicalViewResizing(object musicalObject, double delta)
        {
            Note note = (Note)musicalObject;
            long end = note.Start + note.Length;
            long newEnd = SnapToGrid(end + PixelToTick(delta));

            if (newEnd > note.Start)
            {
                note.Length = newEnd - note.Start;
            }
        }

        public void MusicalViewResizeComplete(object musicalobject)
        {
            ReIndexItem(musicalobject);
        }

        #endregion
    }

    delegate void PianoRollInteractionEventHandler(object sender, PianoRollInteractionEventArgs e);

    class PianoRollInteractionEventArgs : RoutedEventArgs
    {
        public long Position { get; }
        public long SnappedPosition { get; }
        public byte Value { get; }
        public MouseButtonEventArgs MouseButtonEventData { get; }

        public PianoRollInteractionEventArgs(RoutedEvent id, long position, long snappedPosition, byte value, MouseButtonEventArgs mouseEvent)
        {
            RoutedEvent = id;
            Position = position;
            SnappedPosition = snappedPosition;
            Value = value;
            MouseButtonEventData = mouseEvent;
        }
    }
}
