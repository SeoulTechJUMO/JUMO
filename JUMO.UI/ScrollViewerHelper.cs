using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JUMO.UI
{
    class ScrollViewerHelper : DependencyObject
    {
        private static readonly Dictionary<ScrollViewer, object> _svTable = new Dictionary<ScrollViewer, object>();
        private static readonly Dictionary<object, double> _hOffsets = new Dictionary<object, double>();
        private static readonly Dictionary<object, double> _vOffsets = new Dictionary<object, double>();

        public static readonly DependencyProperty SyncGroupProperty =
            DependencyProperty.RegisterAttached(
                "SyncGroup", typeof(object), typeof(ScrollViewerHelper),
                new PropertyMetadata(OnSyncGroupChanged)
            );

        public static readonly DependencyProperty SyncDirectionProperty =
            DependencyProperty.RegisterAttached(
                "SyncDirection", typeof(SyncDirection), typeof(ScrollViewerHelper),
                new PropertyMetadata(SyncDirection.Both)
            );

        public static readonly DependencyProperty ShiftWheelScrollsHorizontallyProperty =
            DependencyProperty.RegisterAttached(
                "ShiftWheelScrollsHorizontally", typeof(bool), typeof(ScrollViewerHelper),
                new PropertyMetadata(false, OnShiftWheelScrollsHorizontallyChanged)
            );

        public static object GetSyncGroup(DependencyObject obj)
            => obj.GetValue(SyncGroupProperty);

        public static void SetSyncGroup(DependencyObject obj, object newGroup)
            => obj.SetValue(SyncGroupProperty, newGroup);

        public static SyncDirection GetSyncDirection(DependencyObject obj)
            => (SyncDirection)obj.GetValue(SyncDirectionProperty);

        public static void SetSyncDirection(DependencyObject obj, SyncDirection newDirection)
            => obj.SetValue(SyncDirectionProperty, newDirection);

        public static bool GetShiftWheelScrollsHorizontally(DependencyObject obj)
            => (bool)obj.GetValue(ShiftWheelScrollsHorizontallyProperty);

        public static void SetShiftWheelScrollsHorizontally(DependencyObject obj, bool value)
            => obj.SetValue(ShiftWheelScrollsHorizontallyProperty, value);

        private static void OnSyncGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ScrollViewer scrollViewer))
            {
                return;
            }

            object oldGroup = e.OldValue;
            object newGroup = e.NewValue;

            if (oldGroup != null)
            {
                if (_svTable.ContainsKey(scrollViewer))
                {
                    scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                    _svTable.Remove(scrollViewer);
                }
            }

            if (newGroup != null)
            {
                SyncDirection direction = GetSyncDirection(scrollViewer);

                if (_hOffsets.ContainsKey(newGroup))
                {
                    if (direction.HasFlag(SyncDirection.Horizontal))
                    {
                        scrollViewer.ScrollToHorizontalOffset(_hOffsets[newGroup]);
                    }
                }
                else
                {
                    _hOffsets.Add(newGroup, scrollViewer.HorizontalOffset);
                }

                if (_vOffsets.ContainsKey(newGroup))
                {
                    if (direction.HasFlag(SyncDirection.Vertical))
                    {
                        scrollViewer.ScrollToVerticalOffset(_vOffsets[newGroup]);
                    }
                }
                else
                {
                    _vOffsets.Add(newGroup, scrollViewer.VerticalOffset);
                }

                _svTable.Add(scrollViewer, newGroup);
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
        }

        private static void OnShiftWheelScrollsHorizontallyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ScrollViewer scrollViewer))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            }
            else
            {
                scrollViewer.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;
            }
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange == 0 && e.VerticalChange == 0)
            {
                return;
            }

            if (!(sender is ScrollViewer changedScrollViewer))
            {
                return;
            }

            object group = _svTable[changedScrollViewer];
            SyncDirection srcDirection = GetSyncDirection(changedScrollViewer);

            if (srcDirection.HasFlag(SyncDirection.Horizontal))
            {
                _hOffsets[group] = changedScrollViewer.HorizontalOffset;
            }

            if (srcDirection.HasFlag(SyncDirection.Vertical))
            {
                _vOffsets[group] = changedScrollViewer.VerticalOffset;
            }

            var affectedScrollViewers =
                from kv in _svTable
                where kv.Value == @group && kv.Key != changedScrollViewer
                select kv.Key;

            foreach (ScrollViewer sv in affectedScrollViewers)
            {
                SyncDirection dir = GetSyncDirection(sv);

                if (dir.HasFlag(SyncDirection.Horizontal)
                    && sv.HorizontalOffset != _hOffsets[group])
                {
                    sv.ScrollToHorizontalOffset(_hOffsets[group]);
                }

                if (dir.HasFlag(SyncDirection.Vertical)
                    && sv.VerticalOffset != _vOffsets[group])
                {
                    sv.ScrollToVerticalOffset(_vOffsets[group]);
                }
            }
        }

        private static void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(sender is ScrollViewer scrollViewer))
            {
                return;
            }

            if (Keyboard.Modifiers != ModifierKeys.Shift)
            {
                return;
            }

            if (e.Delta < 0)
            {
                scrollViewer.LineRight();
            }
            else
            {
                scrollViewer.LineLeft();
            }

            e.Handled = true;
        }

        [Flags]
        public enum SyncDirection
        {
            DoNotSync = 0,
            Horizontal = 1,
            Vertical = 2,
            Both = Horizontal | Vertical
        }
    }
}
