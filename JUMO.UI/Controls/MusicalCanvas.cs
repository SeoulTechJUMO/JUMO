﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using JUMO.UI.Data;

namespace JUMO.UI.Controls
{
    class MusicalCanvas : FrameworkElement, IScrollInfo
    {
        #region Dependency Properties

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
                "Items", typeof(ObservableCollection<INote>), typeof(MusicalCanvas),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender,
                    ItemsPropertyChangedCallback
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

        private int TimeResolution => MusicalProps.GetTimeResolution(this);
        private int GridUnit => MusicalProps.GetGridUnit(this);
        private int ZoomFactor => MusicalProps.GetZoomFactor(this);

        public ObservableCollection<INote> Items
        {
            get => (ObservableCollection<INote>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
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

        private void SetViewport(Size newViewport)
        {
            if (_viewport != newViewport)
            {
                _viewport = newViewport;
                OnScrollChanged();
            }
        }

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
            _transform.X = -offset;
            OnScrollChanged();
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
            _transform.Y = -offset;
            ScrollOwner?.InvalidateScrollInfo();
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect(); // throw new NotImplementedException();
        }

        #endregion

        private double _widthPerTick = 0;
        private BinaryPartition<INote> _index;

        private void CalculateExtent(bool force)
        {
            if (_widthPerTick == 0)
            {
                _widthPerTick = (ZoomFactor << 2) / (double)TimeResolution;
            }

            long totalLength = Items?.Aggregate(0L, (acc, note) => Math.Max(acc, note.Start + note.Length)) ?? 0;
            Size newExtent = new Size(totalLength * _widthPerTick, 2560);

            if (_extent != newExtent)
            {
                _extent = newExtent;
                _index = new BinaryPartition<INote>()
                {
                    Bounds = new Segment(0, totalLength)
                };

                // TODO: do we need to reload all items every time we calculate the extent?
                foreach (INote note in Items)
                {
                    _index.Insert(note, new Segment(note.Start, note.Length));
                }

                SetHorizontalOffset(HorizontalOffset);
                SetVerticalOffset(VerticalOffset);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            CalculateExtent(false);

            if (_viewport != availableSize)
            {
                SetViewport(availableSize);
                SetHorizontalOffset(HorizontalOffset);
                SetVerticalOffset(VerticalOffset);
            }

            /*
             * TODO: Measure children here.
             */
            foreach (UIElement element in _children)
            {
                if (element == null)
                {
                    continue;
                }

                element.Measure(new Size(GetLength(element) * _widthPerTick, 20));
            }
            
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            CalculateExtent(false);

            if (_viewport != finalSize)
            {
                SetViewport(finalSize);
                SetHorizontalOffset(HorizontalOffset);
                SetVerticalOffset(VerticalOffset);
            }

            /*
             * TODO: Arrange children here.
             */
            foreach (UIElement element in _children)
            {
                if (element == null)
                {
                    continue;
                }

                element.Arrange(new Rect(new Point(GetStart(element) * _widthPerTick, (127 - GetNoteValue(element)) * 20), new Size(GetLength(element) * _widthPerTick, 20)));
            }
            
            return finalSize;
        }

        /*protected override Size MeasureOverride(Size availableSize)
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

            bool extentChanged = _extent != newExtent;
            bool viewportChanged = _viewport != availableSize;

            if (extentChanged)
            {
                _extent = newExtent;
            }

            if (viewportChanged)
            {
                _viewport = availableSize;
            }

            if (extentChanged || viewportChanged)
            {
                SetHorizontalOffset(HorizontalOffset);
                SetVerticalOffset(VerticalOffset);
            }

            return availableSize;
        }*/

        /*protected override Size ArrangeOverride(Size finalSize)
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
        }*/

        private void OnScrollChanged()
        {
            Segment visibleTicks = new Segment(HorizontalOffset / _widthPerTick, ViewportWidth / _widthPerTick);
            var visibleNotes = _index.GetItemsInside(visibleTicks);

            _children.Clear();
            foreach (INote note in visibleNotes)
            {
                Rectangle r = new Rectangle()
                {
                    Width = note.Length * _widthPerTick,
                    Height = 20,
                    Fill = Brushes.MediumSpringGreen
                };

                SetNoteValue(r, note.Value);
                SetStart(r, note.Start);
                SetLength(r, note.Length);
                _children.Add(r);
            }

            InvalidateArrange();

            ScrollOwner?.InvalidateScrollInfo();
        }

        #region Visual Host Container Implementation

        private readonly VisualCollection _children;

        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= VisualChildrenCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _children[index];
        }

        #endregion

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            InvalidateArrange();
        }

        private void UnregisterItems(INotifyCollectionChanged collection)
        {
            if (collection != null)
            {
                collection.CollectionChanged -= OnItemsCollectionChanged;
            }
        }

        private void RegisterItems(INotifyCollectionChanged collection)
        {
            if (collection != null)
            {
                collection.CollectionChanged += OnItemsCollectionChanged;
            }
        }

        private void RefreshItems()
        {
            System.Diagnostics.Debug.WriteLine("MusicalCanvas::RefreshItems called");
            InvalidateArrange();
        }

        private static void ItemsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MusicalCanvas ctrl))
            {
                return;
            }

            ctrl.UnregisterItems(e.OldValue as INotifyCollectionChanged);
            ctrl.RegisterItems(e.NewValue as INotifyCollectionChanged);
            ctrl.RefreshItems();
        }

        private static void MusicalPropertiesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MusicalCanvas ctrl))
            {
                return;
            }

            ctrl._widthPerTick = (ctrl.ZoomFactor << 2) / (double)ctrl.TimeResolution;
        }

        static MusicalCanvas()
        {
            MusicalProps.TimeResolutionProperty.OverrideMetadata(
                typeof(MusicalCanvas),
                new FrameworkPropertyMetadata(
                    480,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender,
                    MusicalPropertiesChangedCallback
                )
            );

            MusicalProps.ZoomFactorProperty.OverrideMetadata(
                typeof(MusicalCanvas),
                new FrameworkPropertyMetadata(
                    24,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender,
                    MusicalPropertiesChangedCallback
                )
            );
        }

        public MusicalCanvas()
        {
            _children = new VisualCollection(this);
            RenderTransform = _transform;
        }
    }
}
