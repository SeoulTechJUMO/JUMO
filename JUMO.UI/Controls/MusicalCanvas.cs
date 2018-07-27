using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace JUMO.UI.Controls
{
    class MusicalCanvas : Panel, IScrollInfo
    {
        #region Dependency Properties

        public static readonly DependencyProperty TimeResolutionProperty =
            DependencyProperty.Register(
                "TimeResolution", typeof(int), typeof(MusicalCanvas),
                new FrameworkPropertyMetadata(
                    480,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty GridUnitProperty =
            DependencyProperty.Register(
                "GridUnit", typeof(int), typeof(MusicalCanvas),
                new FrameworkPropertyMetadata(
                    16,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register(
                "ZoomFactor", typeof(int), typeof(MusicalCanvas),
                new FrameworkPropertyMetadata(
                    24,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty NoteValueProperty =
            DependencyProperty.RegisterAttached(
                "NoteValue", typeof(int), typeof(MusicalCanvas),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty StartProperty =
            DependencyProperty.RegisterAttached(
                "Start", typeof(long), typeof(MusicalCanvas),
                new FrameworkPropertyMetadata(
                    0L,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.RegisterAttached(
                "Length", typeof(long), typeof(MusicalCanvas),
                new FrameworkPropertyMetadata(
                    0L,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        #endregion

        #region Dependency Property Accessors

        public int TimeResolution
        {
            get => (int)GetValue(TimeResolutionProperty);
            set => SetValue(TimeResolutionProperty, value);
        }

        public int ZoomFactor
        {
            get => (int)GetValue(ZoomFactorProperty);
            set => SetValue(ZoomFactorProperty, value);
        }

        public int GridUnit
        {
            get => (int)GetValue(GridUnitProperty);
            set => SetValue(GridUnitProperty, value);
        }

        public static int GetNoteValue(UIElement target) => (int)target.GetValue(NoteValueProperty);
        public static void SetNoteValue(UIElement target, int value) => target.SetValue(NoteValueProperty, value);
        public static long GetStart(UIElement target) => (long)target.GetValue(StartProperty);
        public static void SetStart(UIElement target, long value) => target.SetValue(StartProperty, value);
        public static long GetLength(UIElement target) => (long)target.GetValue(LengthProperty);
        public static void SetLength(UIElement target, long value) => target.SetValue(LengthProperty, value);

        #endregion

        #region IScrollInfo Properties

        private Size _extent = new Size(0, 0);
        private Size _viewport = new Size(0, 0);
        private Point _offset = new Point(0, 0);
        private readonly TranslateTransform _transform = new TranslateTransform();

        public bool CanVerticallyScroll { get; set; }
        public bool CanHorizontallyScroll { get; set; }

        public double ExtentWidth => _extent.Width;
        public double ExtentHeight => _extent.Height;
        public double ViewportWidth => _viewport.Width;
        public double ViewportHeight => _viewport.Height;
        public double HorizontalOffset => _offset.X;
        public double VerticalOffset => _offset.Y;

        public ScrollViewer ScrollOwner { get; set; }

        #endregion

        #region IScrollInfo Methods

        public void LineUp() => SetVerticalOffset(VerticalOffset - 3);
        public void LineDown() => SetVerticalOffset(VerticalOffset + 3);
        public void LineLeft() => SetHorizontalOffset(HorizontalOffset - 3);
        public void LineRight() => SetHorizontalOffset(HorizontalOffset + 3);

        public void PageUp() => SetVerticalOffset(VerticalOffset - ViewportHeight / 2);
        public void PageDown() => SetVerticalOffset(VerticalOffset + ViewportHeight / 2);
        public void PageLeft() => SetHorizontalOffset(HorizontalOffset - ViewportWidth / 2);
        public void PageRight() => SetHorizontalOffset(HorizontalOffset + ViewportWidth / 2);

        public void MouseWheelUp() => SetVerticalOffset(VerticalOffset - 20);
        public void MouseWheelDown() => SetVerticalOffset(VerticalOffset + 20);
        public void MouseWheelLeft() => SetHorizontalOffset(HorizontalOffset - ZoomFactor);
        public void MouseWheelRight() => SetHorizontalOffset(HorizontalOffset + ZoomFactor);

        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || ExtentWidth <= ViewportWidth)
            {
                offset = 0;
            }
            else if (offset + ViewportWidth >= ExtentWidth)
            {
                offset = ExtentWidth - ViewportWidth;
            }

            _offset.X = offset;

            ScrollOwner?.InvalidateScrollInfo();

            _transform.X = -offset;
        }

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || ExtentHeight <= ViewportHeight)
            {
                offset = 0;
            }
            else if (offset + ViewportHeight >= ExtentHeight)
            {
                offset = ExtentHeight - ViewportHeight;
            }

            _offset.Y = offset;

            ScrollOwner?.InvalidateScrollInfo();

            _transform.Y = -offset;
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect(); // throw new NotImplementedException();
        }

        #endregion

        protected override Size MeasureOverride(Size availableSize)
        {
            double widthPerTick = (ZoomFactor << 2) / (double)TimeResolution;
            long endTick = 0;

            foreach (UIElement e in InternalChildren)
            {
                long start = GetStart(e);
                long length = GetLength(e);
                long end = start + length;

                e.Measure(availableSize);

                endTick = end > endTick ? end : endTick;
            }

            Size newExtent = new Size(endTick * widthPerTick, 2560);

            if (_extent != newExtent)
            {
                _extent = newExtent;
                ScrollOwner?.InvalidateScrollInfo();
            }

            if (_viewport != availableSize)
            {
                _viewport = availableSize;
                ScrollOwner?.InvalidateScrollInfo();
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double widthPerTick = (ZoomFactor << 2) / (double)TimeResolution;

            foreach (UIElement e in InternalChildren)
            {
                double x = GetStart(e) * widthPerTick;
                double y = (127 - GetNoteValue(e)) * 20;
                double w = GetLength(e) * widthPerTick;

                e.Arrange(new Rect(new Point(x, y), new Size(w, 20)));
            }

            return finalSize;
        }

        public MusicalCanvas()
        {
            RenderTransform = _transform;
        }
    }
}
