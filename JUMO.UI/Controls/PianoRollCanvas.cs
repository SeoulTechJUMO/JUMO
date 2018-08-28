using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JUMO.UI.Controls
{
    class PianoRollCanvas : MusicalCanvasBase
    {
        #region Attached Properties

        public static readonly DependencyProperty NoteValueProperty =
            DependencyProperty.RegisterAttached(
                "NoteValue", typeof(int), typeof(PianoRollCanvas),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty StartProperty =
            DependencyProperty.RegisterAttached(
                "Start", typeof(long), typeof(PianoRollCanvas),
                new FrameworkPropertyMetadata(
                    0L,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.RegisterAttached(
                "Length", typeof(long), typeof(PianoRollCanvas),
                new FrameworkPropertyMetadata(
                    0L,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        #endregion

        #region Attached Property Accessors

        public static int GetNoteValue(UIElement target) => (int)target.GetValue(NoteValueProperty);
        public static void SetNoteValue(UIElement target, int value) => target.SetValue(NoteValueProperty, value);
        public static long GetStart(UIElement target) => (long)target.GetValue(StartProperty);
        public static void SetStart(UIElement target, long value) => target.SetValue(StartProperty, value);
        public static long GetLength(UIElement target) => (long)target.GetValue(LengthProperty);
        public static void SetLength(UIElement target, long value) => target.SetValue(LengthProperty, value);

        #endregion

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
            return new Size(GetLength(element) * WidthPerTick, 20);
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            double x = GetStart(element) * WidthPerTick;
            double y = (127 - GetNoteValue(element)) * 20;
            double w = GetLength(element) * WidthPerTick;

            return new Rect(new Point(x, y), new Size(w, 20));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(this);
            byte value = (byte)(127 - ((int)pt.Y / 20));
            long pos = (long)(pt.X * TimeResolution / (ZoomFactor << 2));
            long gridTick = (long)(TimeResolution * (4.0 / GridUnit));
            long snap = (pos / gridTick) * gridTick;

            RaiseEvent(new PianoRollInteractionEventArgs(InteractionEvent, pos, snap, value, e));
            e.Handled = true;
        }
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
