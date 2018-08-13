using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JUMO.UI.Data
{
    class BinaryPartition<T> where T : class
    {
        private Segment _bounds;
        private Partition _root;
        private readonly IDictionary<T, Partition> _table = new Dictionary<T, Partition>();

        public Segment Bounds
        {
            get => _bounds;
            set
            {
                _bounds = value;
                ReIndex();
            }
        }

        public void Insert(T item, Segment bounds)
        {
            _root = _root ?? new Partition(null, Bounds);
            Partition parent = _root.Insert(item, bounds);
            _table[item] = parent;
        }

        public bool Remove(T item)
        {
            if (_table.TryGetValue(item, out Partition parent))
            {
                parent.Remove(item);
                _table.Remove(item);

                return true;
            }

            return false;
        }

        public IEnumerable<T> GetItemsInside(Segment bounds)
        {
            foreach (var item in GetItems(bounds))
            {
                yield return item.Item1;
            }
        }

        public bool HasItemsInside(Segment bounds) => _root?.HasIntersectingItems(bounds) ?? false;

        private IEnumerable<Tuple<T, Segment>> GetItems(Segment bounds)
        {
            List<Tuple<T, Segment>> result = new List<Tuple<T, Segment>>();

            _root?.GetIntersectingItems(result, bounds);

            return result;
        }

        private void ReIndex()
        {
            var allItems = GetItems(_bounds);
            _root = null;

            foreach (var item in allItems)
            {
                Insert(item.Item1, item.Item2);
            }
        }

        internal class Partition
        {
            public Partition Parent { get; }
            public Segment Bounds { get; }

            private readonly IList<Tuple<T, Segment>> _items = new List<Tuple<T, Segment>>();
            private Partition _left;
            private Partition _right;

            public Partition(Partition parent, Segment bounds)
            {
                Parent = parent;
                Bounds = bounds;
            }

            public Partition Insert(T item, Segment bounds)
            {
                double half = Bounds.Length / 2;

                if (half == 0)
                {
                    half = 1;
                }

                Segment leftSegment = new Segment(Bounds.Start, half);
                Segment rightSegment = new Segment(Bounds.Start + half, half);

                Partition child = null;

                if (leftSegment.Contains(bounds))
                {
                    if (_left == null)
                    {
                        _left = new Partition(this, leftSegment);
                    }
                    child = _left;
                }
                else if (rightSegment.Contains(bounds))
                {
                    if (_right == null)
                    {
                        _right = new Partition(this, rightSegment);
                    }
                    child = _right;
                }

                if (child != null)
                {
                    return child.Insert(item, bounds);
                }
                else
                {
                    _items.Add(Tuple.Create(item, bounds));

                    return this;
                }
            }

            public bool Remove(T item)
            {
                var toBeRemoved = _items.FirstOrDefault(tuple => tuple.Item1 == item);

                if (toBeRemoved == null)
                {
                    return false;
                }
                else
                {
                    return _items.Remove(toBeRemoved);
                }
            }

            public void GetIntersectingItems(List<Tuple<T, Segment>> items, Segment bounds)
            {
                double half = Bounds.Length / 2;

                Segment leftSegment = new Segment(Bounds.Start, half);
                Segment rightSegment = new Segment(Bounds.Start + half, half);

                if (leftSegment.IntersectsWith(bounds) && _left != null)
                {
                    _left.GetIntersectingItems(items, bounds);
                }

                if (rightSegment.IntersectsWith(bounds) && _right != null)
                {
                    _right.GetIntersectingItems(items, bounds);
                }

                var intersectingItems = _items.Where(tuple => tuple.Item2.IntersectsWith(bounds));

                foreach (var item in intersectingItems)
                {
                    items.Add(item);
                }
            }

            public bool HasIntersectingItems(Segment bounds)
            {
                double half = Bounds.Length / 2;

                Segment leftSegment = new Segment(Bounds.Start, half);
                Segment rightSegment = new Segment(Bounds.Start + half, half);

                bool found = false;

                if (leftSegment.IntersectsWith(bounds) && _left != null)
                {
                    found = _left.HasIntersectingItems(bounds);
                }

                if (!found && rightSegment.IntersectsWith(bounds) && _right != null)
                {
                    found = _right.HasIntersectingItems(bounds);
                }

                if (!found)
                {
                    found = _items.Any(tuple => tuple.Item2.IntersectsWith(bounds));
                }

                return found;
            }
        }
    }
}
