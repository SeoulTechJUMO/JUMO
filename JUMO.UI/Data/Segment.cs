using System;

namespace JUMO.UI.Data
{
    /// <summary>
    /// 1차원 공간 상의 유한한 한 구간을 나타냅니다.
    /// </summary>
    struct Segment
    {
        private double _start;
        private double _length;

        /// <summary>
        /// 새로운 Segment 구조체를 생성합니다.
        /// </summary>
        /// <param name="start">구간의 시작 지점</param>
        /// <param name="length">구간의 길이</param>
        public Segment(double start, double length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(length)} cannot be negative.");
            }

            _start = start;
            _length = length;
        }

        /// <summary>
        /// 구간의 시작 지점을 가져오거나 설정합니다.
        /// </summary>
        public double Start
        {
            get { return _start; }
            set
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("Empty segment cannot be modified.");
                }

                _start = value;
            }
        }

        /// <summary>
        /// 구간의 길이를 가져오거나 설정합니다.
        /// </summary>
        public double Length
        {
            get { return _length; }
            set
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("Empty segment cannot be modified.");
                }

                if (value < 0)
                {
                    throw new InvalidOperationException($"{nameof(Length)} cannot be negative.");
                }

                _length = value;
            }
        }

        /// <summary>
        /// 이 구간이 비어 있는 구간이면 true, 그렇지 않으면 false입니다.
        /// "비어 있는 구간"과 "길이가 0인 구간"을 혼동하면 안 됩니다.
        /// </summary>
        public bool IsEmpty => _length < 0;

        /// <summary>
        /// 비어 있는 구간을 가져옵니다.
        /// 비어 있는 구간은 시작 지점이 양의 무한대, 길이가 음의 무한대로 설정되어 있기 때문에
        /// 1차원 공간 상의 어디에도 존재하지 않는 특수한 구간입니다.
        /// </summary>
        public static Segment Empty { get; } = CreateEmptySegment();

        /// <summary>
        /// 구간의 끝 지점을 가져옵니다.
        /// 이 구간이 비어있는 구간인 경우 음의 무한대를 반환합니다.
        /// </summary>
        public double End => IsEmpty ? double.NegativeInfinity : Start + Length;

        /// <summary>
        /// 주어진 지점이 이 구간 안에 포함되는지 여부를 반환합니다.
        /// </summary>
        /// <param name="pos">확인하고자 하는 지점</param>
        /// <returns>pos가 이 구간 안에 포함되면 true, 그렇지 않으면 false를 반환합니다.</returns>
        public bool Contains(double pos) => IsEmpty ? false : (pos >= _start) && (pos - _length <= _start);

        /// <summary>
        /// 주어진 구간이 이 구간 안에 포함되는지 여부를 반환합니다.
        /// </summary>
        /// <param name="segment">확인하고자 하는 구간</param>
        /// <returns>segment가 이 구간 안에 포함되면 true, 그렇지 않으면 false를 반환합니다.</returns>
        public bool Contains(Segment segment)
        {
            if (IsEmpty || segment.IsEmpty)
            {
                return false;
            }

            return Start <= segment.Start && End >= segment.End;
        }

        /// <summary>
        /// 주어진 구간이 이 구간과 겹치는지 여부를 반환합니다.
        /// </summary>
        /// <param name="segment">확인하고자 하는 구간</param>
        /// <returns>segment가 이 구간과 겹치는 부분이 있으면 true, 그렇지 않으면 false를 반환합니다.</returns>
        public bool IntersectsWith(Segment segment)
        {
            if (IsEmpty || segment.IsEmpty)
            {
                return false;
            }

            return segment.Start <= End && segment.End >= Start;
        }

        /// <summary>
        /// 이 구간과 주어진 구간이 겹치는 부분으로 이 구간을 변경합니다.
        /// 두 구간이 겹치지 않는 경우 비어 있는 구간이 됩니다.
        /// </summary>
        public void Intersect(Segment segment)
        {
            if (!IntersectsWith(segment))
            {
                this = Empty;
            }
            else
            {
                double start = Math.Max(Start, segment.Start);
                _length = Math.Max(Math.Min(End, segment.End) - start, 0);
                _start = start;
            }
        }

        /// <summary>
        /// 주어진 두 구간이 서로 겹치는 부분을 반환합니다.
        /// </summary>
        public static Segment Intersect(Segment s1, Segment s2)
        {
            s1.Intersect(s2);
            return s1;
        }

        /// <summary>
        /// 이 구간과 주어진 구간을 모두 포함할 수 있을 만큼 큰 구간으로 이 구간을 변경합니다.
        /// </summary>
        public void Union(Segment segment)
        {
            if (IsEmpty)
            {
                this = segment;
            }
            else if (!segment.IsEmpty)
            {
                double start = Math.Min(Start, segment.Start);

                if ((segment.Length == double.PositiveInfinity) || (Length == double.PositiveInfinity))
                {
                    _length = double.PositiveInfinity;
                }
                else
                {
                    double maxEnd = Math.Max(End, segment.End);
                    _length = Math.Max(maxEnd - start, 0);
                }

                _start = start;
            }
        }

        /// <summary>
        /// 주어진 두 구간을 모두 포함할 수 있을 만큼 큰 구간을 반환합니다.
        /// </summary>
        public static Segment Union(Segment s1, Segment s2)
        {
            s1.Union(s2);
            return s1;
        }

        private static Segment CreateEmptySegment()
        {
            Segment s = new Segment
            {
                _start = double.PositiveInfinity,
                _length = double.NegativeInfinity
            };

            return s;
        }

        public static bool operator ==(Segment s1, Segment s2) => s1.Start == s2.Start && s1.Length == s2.Length;
        public static bool operator !=(Segment s1, Segment s2) => !(s1 == s2);

        public static bool Equals(Segment s1, Segment s2) => s1.Start.Equals(s2.Start) && s1.Length.Equals(s2.Length);

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Segment segment)
            {
                return Equals(this, segment);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Segment segment) => Equals(this, segment);
        public override int GetHashCode() => Start.GetHashCode() ^ Length.GetHashCode();
    }
}
