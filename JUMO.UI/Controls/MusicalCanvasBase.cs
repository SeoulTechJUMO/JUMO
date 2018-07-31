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

    class MusicalCanvasBase : FrameworkElement, IScrollInfo
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

        #endregion

        #region Dependency Property Accessors

        private int TimeResolution => MusicalProps.GetTimeResolution(this);
        private int GridUnit => MusicalProps.GetGridUnit(this);
        private int ZoomFactor => MusicalProps.GetZoomFactor(this);

        public IEnumerable Items
        {
            get => (IEnumerable)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
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
        private Segment _visible = Segment.Empty;
        private readonly IList<Segment> _visibleRegions = new List<Segment>();
        private readonly IList<Segment> _dirtyRegions = new List<Segment>();
        private BinaryPartition<IVirtualElement> _index = new BinaryPartition<IVirtualElement>();
        private ObservableCollection<IVirtualElement> _virtualChildren;
        private VirtualElementActivator _elementActivator;
        private DispatcherTimer _timer;
        private readonly SelfThrottlingWorker _createWorker;
        private readonly SelfThrottlingWorker _disposeWorker;
        private bool _isAllCreated = true;

        public VirtualElementActivator ElementActivator
        {
            get => _elementActivator;
            set
            {
                _elementActivator = value;
                RefreshItems();
            }
        }

        private void CalculateExtent(bool force)
        {
            if (_widthPerTick == 0)
            {
                _widthPerTick = (ZoomFactor << 2) / (double)TimeResolution;
            }

            long totalLength = (Items as IEnumerable<INote>)?.Aggregate(0L, (acc, note) => Math.Max(acc, note.Start + note.Length)) ?? 0;
            Size newExtent = new Size((totalLength + (TimeResolution << 3)) * _widthPerTick, 2560);

            if (_extent != newExtent)
            {
                _extent = newExtent;
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

                element.Measure(new Size(PianoRollCanvas.GetLength(element) * _widthPerTick, 20));
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

                double x = PianoRollCanvas.GetStart(element) * _widthPerTick;
                double y = (127 - PianoRollCanvas.GetNoteValue(element)) * 20;
                double w = PianoRollCanvas.GetLength(element) * _widthPerTick;

                element.Arrange(new Rect(new Point(x, y), new Size(w, 20)));
            }

            return finalSize;
        }

        private void OnScrollChanged()
        {
            Segment dirty = _visible;
            _visible = new Segment(HorizontalOffset / _widthPerTick, ViewportWidth / _widthPerTick);
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
            // Eagarly create and destroy visuals (!!!)
            /*foreach (var d in dirtyTicks)
            {
                var removedNotes = _index.GetItemsInside(d);
                foreach (IVirtualElement ve in removedNotes)
                {
                    if (!ve.Bounds.IntersectsWith(_visible))
                    {
                        _children.Remove(ve.Visual);
                        ve.DisposeVisual();
                    }
                }
            }*/

            /*var visibleNotes = _index.GetItemsInside(_visible);
            foreach (IVirtualElement ve in visibleNotes)
            {
                if (ve.Visual == null)
                {
                    ve.CreateVisual(this);
                    _children.Add(ve.Visual);
                }
            }*/

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
                _visible = new Segment(HorizontalOffset / _widthPerTick, ViewportWidth / _widthPerTick);
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
                        _children.Add(ve.Visual);
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
            Segment visible = new Segment(HorizontalOffset / _widthPerTick, ViewportWidth / _widthPerTick);
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
                        _children.Remove(visual);
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

            _virtualChildren = new ObservableCollection<IVirtualElement>();
            if (ElementActivator != null)
            {
                foreach (INote note in Items)
                {
                    _virtualChildren.Add(ElementActivator(note));
                }
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

        private static void MusicalPropertiesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MusicalCanvasBase ctrl)
            {
                ctrl._widthPerTick = (ctrl.ZoomFactor << 2) / (double)ctrl.TimeResolution;
            }
        }

        static MusicalCanvasBase()
        {
            MusicalProps.TimeResolutionProperty.OverrideMetadata(
                typeof(MusicalCanvasBase),
                new FrameworkPropertyMetadata(
                    480,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender,
                    MusicalPropertiesChangedCallback
                )
            );

            MusicalProps.ZoomFactorProperty.OverrideMetadata(
                typeof(MusicalCanvasBase),
                new FrameworkPropertyMetadata(
                    24,
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender,
                    MusicalPropertiesChangedCallback
                )
            );
        }

        public MusicalCanvasBase() : base()
        {
            _children = new VisualCollection(this);
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Normal, OnDispatcherTimerTick, Dispatcher);
            _createWorker = new SelfThrottlingWorker(1000, 50, CreateHandler);
            _disposeWorker = new SelfThrottlingWorker(2000, 50, DisposeHandler);

            RenderTransform = _transform;
        }
    }
}
