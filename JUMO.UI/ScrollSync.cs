using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JUMO.UI
{
    class ScrollSync : DependencyObject
    {
        private static readonly Dictionary<ScrollViewer, object> _svTable = new Dictionary<ScrollViewer, object>();
        private static readonly Dictionary<object, double> _hOffsets = new Dictionary<object, double>();
        private static readonly Dictionary<object, double> _vOffsets = new Dictionary<object, double>();

        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.RegisterAttached(
                "Group", typeof(object), typeof(ScrollSync),
                new PropertyMetadata(OnGroupChanged)
            );

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.RegisterAttached(
                "Direction", typeof(SyncDirection), typeof(ScrollSync),
                new PropertyMetadata(SyncDirection.Both)
            );

        public static object GetGroup(DependencyObject obj) => obj.GetValue(GroupProperty);
        public static void SetGroup(DependencyObject obj, object newGroup) => obj.SetValue(GroupProperty, newGroup);

        public static SyncDirection GetDirection(DependencyObject obj) => (SyncDirection)obj.GetValue(DirectionProperty);
        public static void SetDirection(DependencyObject obj, SyncDirection newDirection) => obj.SetValue(DirectionProperty, newDirection);

        private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
                SyncDirection direction = GetDirection(scrollViewer);

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
            SyncDirection srcDirection = GetDirection(changedScrollViewer);

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
                SyncDirection dir = GetDirection(sv);

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
