using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JUMO.UI.Data;
using JUMO.UI.Views;

namespace JUMO.UI.Controls
{
    abstract class InteractiveMusicalCanvas : MusicalCanvasBase, IMusicalViewCallback
    {
        private const double FOLLOW_ACCEL = 0.0625;

        private readonly List<IVirtualElement> _selectedElements = new List<IVirtualElement>();
        private readonly BlockSelectionHelper _selectionHelper;

        #region Dependency Properties

        public static readonly DependencyProperty GridStepProperty =
            DependencyProperty.Register(
                "GridStep", typeof(int), typeof(InteractiveMusicalCanvas),
                new FrameworkPropertyMetadata(4)
            );

        public static readonly DependencyProperty SnapToGridProperty =
            DependencyProperty.Register(
                "SnapToGrid", typeof(bool), typeof(InteractiveMusicalCanvas),
                new FrameworkPropertyMetadata(true)
            );

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                "SelectedItems", typeof(IEnumerable), typeof(InteractiveMusicalCanvas),
                new FrameworkPropertyMetadata(
                    Enumerable.Empty<IMusicalItem>(),
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    SelectedItemsPropertyChangedCallback
                )
            );

        #endregion

        #region Properties

        public int GridStep
        {
            get => (int)GetValue(GridStepProperty);
            set => SetValue(GridStepProperty, value);
        }

        public bool SnapToGrid
        {
            get => (bool)GetValue(SnapToGridProperty);
            set => SetValue(SnapToGridProperty, value);
        }

        public IEnumerable SelectedItems
        {
            get => (IEnumerable)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<NotifyCollectionChangedEventArgs> SelectionChanged;

        #endregion

        #region Protected Methods

        protected long PixelToTick(double xPos) => (long)(xPos / WidthPerTick);

        protected long SnapToGridInternal(long pos)
        {
            if (!SnapToGrid)
            {
                return pos;
            }

            int ticksPerBeat = (TimeResolution << 2) / MusicalProps.GetDenominator(this);
            int ticksPerGrid = ticksPerBeat / GridStep;
            return (pos / ticksPerGrid) * ticksPerGrid;
        }

        protected void SelectItem(object item)
        {
            SelectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        protected void SelectItems(IList items)
        {
            SelectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        }

        protected void DeselectItem(object item)
        {
            SelectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        protected void DeselectItems(IList items)
        {
            SelectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
        }

        protected void ClearSelection()
        {
            SelectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void FollowMouse()
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

        #endregion

        #region Virtual Properties

        protected abstract int MinVerticalValue { get; }
        protected abstract int MaxVerticalValue { get; }

        #endregion

        #region Virtual Methods

        protected abstract int GetVerticalValue(IMusicalItem item);
        protected virtual int FromVerticalPosition(double y) => (int)y;
        protected virtual int FromVerticalDelta(double dy) => FromVerticalPosition(dy);
        protected virtual void OnSurfaceClick(Point pt) { }

        #endregion

        #region IMusicalViewCallback Members

        public virtual void MusicalViewResizeStarted(FrameworkElement view) { }
        public virtual void MusicalViewResizing(FrameworkElement view, double delta) { }
        public virtual void MusicalViewResizeComplete(FrameworkElement view) { }
        public virtual void MusicalViewMoveStarted(FrameworkElement view) { }
        public virtual void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY) { }
        public virtual void MusicalViewMoveComplete(FrameworkElement view) { }
        public virtual void MusicalViewLeftButtonDown(FrameworkElement view) { }
        public virtual void MusicalViewRightButtonDown(FrameworkElement view) { }

        #endregion

        #region MusicalCanvasBase Overrides

        protected override void OnItemRemoved(IMusicalItem oldItem)
        {
            IVirtualElement ve = LookupVirtualElement(oldItem);

            if (ve != null)
            {
                _selectedElements.Remove(ve);
            }
        }

        protected override void OnItemsReset()
        {
            _selectedElements.Clear();
        }

        #endregion

        #region UIElement Overrides

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(this);

            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                OnSurfaceClick(pt);

                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ClearSelection();
                _selectionHelper.StartBlockSelection(pt);

                e.Handled = true;
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                _selectionHelper.StartBlockSelection(pt);

                e.Handled = true;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_selectionHelper.IsBlockSelecting)
            {
                Rect selectionRect = _selectionHelper.EndBlockSelection();

                long startTick = Math.Max(0L, PixelToTick(selectionRect.Left));
                long length = PixelToTick(selectionRect.Width);
                int vLimit1 = Math.Max(MinVerticalValue, Math.Min(FromVerticalPosition(selectionRect.Top), MaxVerticalValue));
                int vLimit2 = Math.Max(MinVerticalValue, Math.Min(FromVerticalPosition(selectionRect.Bottom), MaxVerticalValue));

                var selectedItems =
                    GetVirtualElementsInside(new Segment(startTick, length))
                    .Select(ve => (IMusicalItem)((SelectableView)ve.Visual).DataContext)
                    .Where(x =>
                    {
                        int v = GetVerticalValue(x);
                        return v >= Math.Min(vLimit1, vLimit2) && v <= Math.Max(vLimit1, vLimit2);
                    });

                SelectItems(new List<IMusicalItem>(selectedItems));

                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_selectionHelper.IsBlockSelecting)
            {
                _selectionHelper.UpdateBlockSelection(e.GetPosition(this));
                FollowMouse();

                e.Handled = true;
            }
        }

        #endregion

        #region Internal Methods

        private void SelectItemInternal(IMusicalItem item)
        {
            IVirtualElement ve = LookupVirtualElement(item);

            if (ve != null)
            {
                _selectedElements.Add(ve);
                ve.IsSelected = true;
            }
        }

        private void DeselectItemInternal(IMusicalItem item)
        {
            IVirtualElement ve = LookupVirtualElement(item);

            if (ve != null)
            {
                _selectedElements.Remove(ve);
                ve.IsSelected = false;
            }
        }

        private void ResetSelectionInternal()
        {
            foreach (var ve in _selectedElements)
            {
                ve.IsSelected = false;
            }

            _selectedElements.Clear();
        }

        #endregion

        #region Callbacks

        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (IMusicalItem newItem in e.NewItems)
                {
                    SelectItemInternal(newItem);
                }
            }

            if (e.OldItems != null)
            {
                foreach (IMusicalItem oldItem in e.OldItems)
                {
                    DeselectItemInternal(oldItem);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ResetSelectionInternal();
            }
        }

        private void OnSelectedItemsPropertyChanged(IEnumerable oldCollection, IEnumerable newCollection)
        {
            if (oldCollection is INotifyCollectionChanged oldIncc)
            {
                oldIncc.CollectionChanged -= OnSelectedItemsCollectionChanged;
            }

            if (newCollection is INotifyCollectionChanged newIncc)
            {
                newIncc.CollectionChanged += OnSelectedItemsCollectionChanged;
            }

            ResetSelectionInternal();

            if (SelectedItems != null)
            {
                foreach (IMusicalItem item in SelectedItems)
                {
                    SelectItemInternal(item);
                }
            }
        }

        private static void SelectedItemsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InteractiveMusicalCanvas ctrl)
            {
                ctrl.OnSelectedItemsPropertyChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable);
            }
        }

        #endregion

        public InteractiveMusicalCanvas() : base()
        {
            _selectionHelper = new BlockSelectionHelper(this);
        }
    }
}
