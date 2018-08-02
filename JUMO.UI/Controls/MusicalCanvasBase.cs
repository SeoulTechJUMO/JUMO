using System;
using System.Collections;
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
using System.Windows.Threading;
using JUMO.UI.Data;

namespace JUMO.UI.Controls
{
    interface IVirtualElement
    {
        Segment Bounds { get; }
        UIElement Visual { get; }
        UIElement CreateVisual(MusicalCanvasBase parent);
        void DisposeVisual();
        event EventHandler BoundsChanged;
    }

    // TODO: 완성되면 별도의 파일로 분리할 것.
    class VirtualNote : IVirtualElement
    {
        private readonly INote _note;

        public VirtualNote(INote note)
        {
            _note = note;
            Bounds = new Segment(_note.Start, _note.Length);
        }

        public Segment Bounds { get; }

        public UIElement Visual { get; private set; }

        public event EventHandler BoundsChanged;

        public UIElement CreateVisual(MusicalCanvasBase parent)
        {
            if (Visual == null)
            {
                /*Rectangle r = new Rectangle()
                {
                    Fill = Brushes.MediumSpringGreen
                };*/
                Button r = new Button()
                {
                    Content = $"{_note.Value}",
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };

                PianoRollCanvas.SetNoteValue(r, _note.Value);
                PianoRollCanvas.SetStart(r, _note.Start);
                PianoRollCanvas.SetLength(r, _note.Length);

                Visual = r;
            }

            return Visual;
        }

        public void DisposeVisual()
        {
            Visual = null;
        }
    }

    class VirtualVelocityControl : IVirtualElement
    {
        private readonly INote _note;

        public VirtualVelocityControl(INote note)
        {
            _note = note;
            Bounds = new Segment(_note.Start, _note.Length);
        }

        public Segment Bounds { get; }

        public UIElement Visual { get; private set; }

        public event EventHandler BoundsChanged;

        public UIElement CreateVisual(MusicalCanvasBase parent)
        {
            if (Visual == null)
            {
                Border b1 = new Border()
                {
                    BorderBrush = Brushes.Fuchsia,
                    BorderThickness = new Thickness(0, 1.5, 0, 0),
                    Background = new SolidColorBrush(Colors.Fuchsia) { Opacity = 0.125 }
                };

                Border b2 = new Border()
                {
                    BorderBrush = Brushes.Fuchsia,
                    BorderThickness = new Thickness(1, 0, 0, 0)
                };

                Ellipse e = new Ellipse()
                {
                    Stroke = Brushes.Fuchsia,
                    StrokeThickness = 1.5,
                    Fill = Brushes.White,
                    Width = 6.0,
                    Height = 6.0,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(-4, -3.5, 0, 0)
                };

                b2.Child = e;
                b1.Child = b2;

                VelocityCanvas.SetVelocity(b1, _note.Velocity);
                VelocityCanvas.SetStart(b1, _note.Start);
                VelocityCanvas.SetLength(b1, _note.Length);

                Visual = b1;
            }

            return Visual;
        }

        public void DisposeVisual()
        {
            Visual = null;
        }
    }

    abstract class MusicalCanvasBase : FrameworkElement, IScrollInfo
    {
        #region Dependency Properties

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
                "Items", typeof(IEnumerable), typeof(MusicalCanvasBase),
                new FrameworkPropertyMetadata(
                    Enumerable.Empty<object>(),
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
        protected int GridUnit => MusicalProps.GetGridUnit(this);
        protected int ZoomFactor => MusicalProps.GetZoomFactor(this);

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
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect(); // throw new NotImplementedException();
        }

        #endregion

        private Segment _visible = Segment.Empty;
        private readonly IList<Segment> _visibleRegions = new List<Segment>();
        private readonly IList<Segment> _dirtyRegions = new List<Segment>();

        private BinaryPartition<IVirtualElement> _index = new BinaryPartition<IVirtualElement>();
        private ObservableCollection<IVirtualElement> _virtualChildren = new ObservableCollection<IVirtualElement>();

        private DispatcherTimer _timer;
        private readonly SelfThrottlingWorker _createWorker;
        private readonly SelfThrottlingWorker _disposeWorker;
        private bool _isAllCreated = true;

        protected double WidthPerTick { get; private set; } = 0;

        protected abstract IVirtualElement CreateVirtualElementForItem(object item);
        protected abstract double CalculateLogicalLength();
        protected abstract Size CalculateSizeForElement(UIElement element);
        protected abstract Rect CalculateRectForElement(UIElement element);

        private void CalculateExtent(bool force, Size availableSize)
        {
            // TODO: 스크롤하거나 확대/축소 비율을 변경하는 경우에는
            //       논리 공간의 길이가 변하지 않으므로 매번 계산할 필요가 없음. (* 1)
            //
            //   논리 공간의 길이를 다시 계산해야 하는 경우:
            //   1. 오른쪽 맨 끝에 항목이 추가될 때
            //   2. 오른쪽 맨 끝에 있는 항목이 제거될 때
            //   3. Items 속성이 다른 컬렉션의 인스턴스로 변경될 때
            double totalLength = CalculateLogicalLength();
            double extentHeight = double.IsNaN(ExtentHeightOverride) ? availableSize.Height : ExtentHeightOverride;
            Size newExtent = new Size(totalLength * WidthPerTick, extentHeight);

            if (_extent != newExtent)
            {
                _extent = newExtent;

                // TODO: (* 1)
                _index = new BinaryPartition<IVirtualElement>()
                {
                    Bounds = new Segment(0, totalLength)
                };

                // TODO: do we need to reload all items every time we calculate the extent?
                foreach (IVirtualElement ve in _virtualChildren)
                {
                    _index.Insert(ve, ve.Bounds);
                }

                SetHorizontalOffset(HorizontalOffset);
                SetVerticalOffset(VerticalOffset);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            WidthPerTick = (ZoomFactor << 2) / (double)TimeResolution;

            CalculateExtent(false, availableSize);

            if (_viewport != availableSize)
            {
                SetViewport(availableSize);
                SetHorizontalOffset(HorizontalOffset);
                SetVerticalOffset(VerticalOffset);
            }

            foreach (UIElement element in Children)
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
            // CalculateExtent(false);

            if (_viewport != finalSize)
            {
                SetViewport(finalSize);
                SetHorizontalOffset(HorizontalOffset);
                SetVerticalOffset(VerticalOffset);
            }

            foreach (UIElement element in Children)
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
            /*if (_index != null)
            {
                CalculateExtent(false);
            }*/

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

            _virtualChildren = new ObservableCollection<IVirtualElement>();
            foreach (object item in Items)
            {
                _virtualChildren.Add(CreateVirtualElementForItem(item));
            }

            InvalidateArrange();
        }

        private static void ItemsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MusicalCanvasBase ctrl)
            {
                ctrl.UnregisterItems(e.OldValue as INotifyCollectionChanged);
                ctrl.RegisterItems(e.NewValue as INotifyCollectionChanged);
                ctrl.RefreshItems();
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
