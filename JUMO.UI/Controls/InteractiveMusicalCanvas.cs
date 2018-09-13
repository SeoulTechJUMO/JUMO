using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace JUMO.UI.Controls
{
    abstract class InteractiveMusicalCanvas : MusicalCanvasBase
    {
        private const double FOLLOW_ACCEL = 0.0625;

        private readonly List<IVirtualElement> _selectedElements = new List<IVirtualElement>();

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

        #region Overrides

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
    }
}
