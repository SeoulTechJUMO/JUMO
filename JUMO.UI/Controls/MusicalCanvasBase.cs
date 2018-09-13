using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using JUMO.UI.Data;

namespace JUMO.UI.Controls
{
    abstract class MusicalCanvasBase : FrameworkElement, IScrollInfo
    {
        #region Dependency Properties

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
                "Items", typeof(IEnumerable), typeof(MusicalCanvasBase),
                new FrameworkPropertyMetadata(
                    Enumerable.Empty<IMusicalItem>(),
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender,
                    ItemsPropertyChangedCallback
                )
            );

        public static readonly DependencyProperty ExtentHeightOverrideProperty =
            DependencyProperty.Register(
                "ExtentHeightOverride", typeof(double), typeof(MusicalCanvasBase),
                new FrameworkPropertyMetadata(
                    double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        #endregion

        #region Dependency Property Accessors

        protected int TimeResolution => MusicalProps.GetTimeResolution(this);
        protected double ZoomFactor => MusicalProps.GetZoomFactor(this);

        public IEnumerable Items
        {
            get => (IEnumerable)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public double ExtentHeightOverride
        {
            get => (double)GetValue(ExtentHeightOverrideProperty);
            set => SetValue(ExtentHeightOverrideProperty, value);
        }

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
        public void LineLeft() => SetHorizontalOffset(HorizontalOffset - ZoomFactor);
        public void LineRight() => SetHorizontalOffset(HorizontalOffset + ZoomFactor);

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
            InvalidateVisual();
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect(); // throw new NotImplementedException();
        }

        #endregion

        private double _logicalLength = 0;
        private Segment _visible = Segment.Empty;
        private readonly IList<Segment> _visibleRegions = new List<Segment>();
        private readonly IList<Segment> _dirtyRegions = new List<Segment>();

        private BinaryPartition<IVirtualElement> _index = new BinaryPartition<IVirtualElement>();
        private readonly Dictionary<IMusicalItem, IVirtualElement> _table = new Dictionary<IMusicalItem, IVirtualElement>();

        private DispatcherTimer _timer;
        private readonly SelfThrottlingWorker _createWorker;
        private readonly SelfThrottlingWorker _disposeWorker;
        private bool _isAllCreated = true;

        protected double WidthPerTick { get; private set; } = 0;

        protected abstract IVirtualElement CreateVirtualElementForItem(IMusicalItem item);
        protected abstract double CalculateLogicalLength();
        protected abstract Size CalculateSizeForElement(FrameworkElement element);
        protected abstract Rect CalculateRectForElement(FrameworkElement element);

        protected IVirtualElement LookupVirtualElement(IMusicalItem item)
        {
            _table.TryGetValue(item, out IVirtualElement ve);

            return ve;
        }

        protected IEnumerable<IVirtualElement> GetVirtualElementsInside(Segment bounds)
        {
            foreach (IVirtualElement ve in _index.GetItemsInside(bounds))
            {
                yield return ve;
            }
        }

        protected virtual void OnItemAdded(IMusicalItem newItem) { }
        protected virtual void OnItemRemoved(IMusicalItem oldItem) { }
        protected virtual void OnItemsReset() { }

        private void CalculateLogicalLengthInternal()
        {
            double newLogicalLength = CalculateLogicalLength();

            if (_logicalLength != newLogicalLength)
            {
                _logicalLength = newLogicalLength;
                _index.Bounds = new Segment(0, _logicalLength);
            }

            InvalidateMeasure();
        }

        private void ScaleExtent()
        {
            _extent.Width = _logicalLength * WidthPerTick;
            _extent.Height = double.IsNaN(ExtentHeightOverride) ? ActualHeight : ExtentHeightOverride;
            SetHorizontalOffset(HorizontalOffset);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            WidthPerTick = ZoomFactor * 4 / TimeResolution;

            ScaleExtent();

            if (_viewport != availableSize)
            {
                SetViewport(availableSize);
                SetHorizontalOffset(HorizontalOffset);
            }

            foreach (FrameworkElement element in Children)
            {
                if (element == null)
                {
                    continue;
                }

                element.Measure(CalculateSizeForElement(element));
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_viewport != finalSize)
            {
                SetViewport(finalSize);
                SetHorizontalOffset(HorizontalOffset);
            }

            foreach (FrameworkElement element in Children)
            {
                if (element == null)
                {
                    continue;
                }

                element.Arrange(CalculateRectForElement(element));
            }

            return finalSize;
        }

        private void OnScrollChanged()
        {
            Segment dirty = _visible;
            _visible = new Segment(HorizontalOffset / WidthPerTick, ViewportWidth / WidthPerTick);
            _visibleRegions.Clear();
            _visibleRegions.Add(_visible);
            Segment intersection = Segment.Intersect(dirty, _visible);

            if (intersection.IsEmpty)
            {
                _dirtyRegions.Add(dirty);
            }
            else
            {
                if (dirty.Start < intersection.Start)
                {
                    _dirtyRegions.Add(new Segment(dirty.Start, intersection.Start - dirty.Start));
                }

                if (dirty.End > intersection.End)
                {
                    _dirtyRegions.Add(new Segment(intersection.End, dirty.End - intersection.End));
                }
            }

            _timer.Start();

            ScrollOwner?.InvalidateScrollInfo();
        }

        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();

            _isAllCreated = true;
            _createWorker.RunWorker();
            _disposeWorker.RunWorker();

            if (!_isAllCreated)
            {
                System.Diagnostics.Debug.WriteLine("not done yet. restarting the timer.");
                _timer.Start();
            }

            InvalidateVisual();
        }

        private int CreateHandler(int quantum)
        {
            if (_visible.IsEmpty)
            {
                _visible = new Segment(HorizontalOffset / WidthPerTick, ViewportWidth / WidthPerTick);
                _visibleRegions.Add(_visible);
                _isAllCreated = false;
            }

            int count = 0;
            int regionCount = 0;

            while (_visibleRegions.Count > 0 && count < quantum)
            {
                Segment s = _visibleRegions[0];
                _visibleRegions.RemoveAt(0);
                regionCount++;

                var ves = _index.GetItemsInside(s);

                foreach (IVirtualElement ve in ves)
                {
                    if (ve.Visual == null)
                    {
                        ve.CreateVisual(this);
                        Children.Add(ve.Visual);
                    }

                    count++;

                    if (count >= quantum)
                    {
                        if (regionCount == 1)
                        {
                            double half = s.Length / 2;
                            _visibleRegions.Add(new Segment(s.Start, half + 24));
                            _visibleRegions.Add(new Segment(s.Start + half, half + 24));
                        }
                        else
                        {
                            _visibleRegions.Add(s);
                        }

                        _isAllCreated = false;
                        break;
                    }
                }
            }

            return count;
        }

        private int DisposeHandler(int quantum)
        {
            Segment visible = new Segment(HorizontalOffset / WidthPerTick, ViewportWidth / WidthPerTick);
            int count = 0;
            int regionCount = 0;

            while (_dirtyRegions.Count > 0 && count < quantum)
            {
                int last = _dirtyRegions.Count - 1;
                Segment dirty = _dirtyRegions[last];
                _dirtyRegions.RemoveAt(last);
                regionCount++;

                var ves = _index.GetItemsInside(dirty);

                foreach (IVirtualElement ve in ves)
                {
                    UIElement visual = ve.Visual;
                    Segment bounds = ve.Bounds;

                    if (visual != null && !bounds.IntersectsWith(visible))
                    {
                        Children.Remove(visual);
                        ve.DisposeVisual();
                    }

                    count++;

                    if (count >= quantum)
                    {
                        if (regionCount == 1)
                        {
                            double half = dirty.Length / 2;
                            _dirtyRegions.Add(new Segment(dirty.Start, half + 24));
                            _dirtyRegions.Add(new Segment(dirty.Start + half, half + 24));
                        }
                        else
                        {
                            _dirtyRegions.Add(dirty);
                        }

                        _isAllCreated = false;
                        break;
                    }
                }
            }

            return count;
        }

        protected override void OnRender(DrawingContext dc)
        {
            Point pt = new Point(-_transform.X, -_transform.Y);
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(pt, RenderSize));
        }

        #region Visual Host Container Implementation

        private VisualCollection _children;

        protected VisualCollection Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new VisualCollection(this);
                }

                return _children;
            }
        }

        protected override int VisualChildrenCount => Children.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= VisualChildrenCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Children[index];
        }

        #endregion

        private void OnVirtualElementBoundsChanged(object sender, EventArgs e)
        {
            if (sender is IVirtualElement ve)
            {
                if (_index.Remove(ve))
                {
                    _index.Insert(ve, ve.Bounds);
                    CalculateLogicalLengthInternal();
                }
            }
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (IMusicalItem newItem in e.NewItems)
                {
                    AddItemInternal(newItem);
                }
            }

            if (e.OldItems != null)
            {
                foreach (IMusicalItem oldItem in e.OldItems)
                {
                    RemoveItemInternal(oldItem);
                }
            }

            CalculateLogicalLengthInternal();
            OnScrollChanged();
        }

        private void OnItemsPropertyChanged(IEnumerable oldCollection, IEnumerable newCollection)
        {
            if (oldCollection is INotifyCollectionChanged oldIncc)
            {
                oldIncc.CollectionChanged -= OnItemsCollectionChanged;
            }

            if (newCollection is INotifyCollectionChanged newIncc)
            {
                newIncc.CollectionChanged += OnItemsCollectionChanged;
            }

            ResetItemsInternal();

            if (Items != null)
            {
                foreach (IMusicalItem item in Items)
                {
                    AddItemInternal(item);
                }
            }

            CalculateLogicalLengthInternal();
        }

        private void AddItemInternal(IMusicalItem item)
        {
            IVirtualElement ve = CreateVirtualElementForItem(item);
            _table.Add(item, ve);
            _index.Insert(ve, ve.Bounds);
            ve.BoundsChanged += OnVirtualElementBoundsChanged;
        }

        private void RemoveItemInternal(IMusicalItem item)
        {
            if (_table.TryGetValue(item, out IVirtualElement ve))
            {
                ve.BoundsChanged -= OnVirtualElementBoundsChanged;

                Children.Remove(ve.Visual);
                ve.DisposeVisual();
                _table.Remove(item);
                _index.Remove(ve);
            }

            OnItemRemoved(item);
        }

        private void ResetItemsInternal()
        {
            foreach (var ve in _table.Values)
            {
                ve.BoundsChanged -= OnVirtualElementBoundsChanged;
            }

            Children.Clear();
            _table.Clear();
            _index = new BinaryPartition<IVirtualElement>();

            OnItemsReset();
        }

        private static void ItemsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MusicalCanvasBase ctrl)
            {
                ctrl.OnItemsPropertyChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable);
            }
        }

        public MusicalCanvasBase() : base()
        {
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Normal, OnDispatcherTimerTick, Dispatcher);
            _createWorker = new SelfThrottlingWorker(1000, 50, CreateHandler);
            _disposeWorker = new SelfThrottlingWorker(2000, 50, DisposeHandler);

            RenderTransform = _transform;
        }
    }
}
