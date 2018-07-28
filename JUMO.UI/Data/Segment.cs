namespace JUMO.UI.Data
{
    /// <summary>
    /// 1차원 공간 상의 유한한 한 구간을 나타냅니다.
    /// </summary>
    struct Segment
    {
        /// <summary>
        /// 새로운 Segment 구조체를 생성합니다.
        /// </summary>
        /// <param name="start">구간의 시작 지점</param>
        /// <param name="length">구간의 길이</param>
        public Segment(double start, double length)
        {
            Start = start;
            Length = length;
        }

        /// <summary>
        /// 구간의 시작 지점을 가져오거나 설정합니다.
        /// </summary>
        public double Start { get; set; }

        /// <summary>
        /// 구간의 길이를 가져오거나 설정합니다.
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// 구간의 끝 지점을 가져오거나 설정합니다.
        /// </summary>
        public double End => Start + Length;

        /// <summary>
        /// 주어진 지점이 이 구간 안에 포함되는지 여부를 반환합니다.
        /// </summary>
        /// <param name="pos">확인하고자 하는 지점</param>
        /// <returns>pos가 이 구간 안에 포함되면 true, 그렇지 않으면 false를 반환합니다.</returns>
        public bool Contains(double pos) => pos >= Start && pos <= End;

        /// <summary>
        /// 주어진 구간이 이 구간 안에 포함되는지 여부를 반환합니다.
        /// </summary>
        /// <param name="segment">확인하고자 하는 구간</param>
        /// <returns>segment가 이 구간 안에 포함되면 true, 그렇지 않으면 false를 반환합니다.</returns>
        public bool Contains(Segment segment) => segment.Start >= Start && segment.End <= End;

        /// <summary>
        /// 주어진 구간이 이 구간과 겹치는지 여부를 반환합니다.
        /// </summary>
        /// <param name="segment">확인하고자 하는 구간</param>
        /// <returns>segment가 이 구간과 겹치는 부분이 있으면 true, 그렇지 않으면 false를 반환합니다.</returns>
        public bool IntersectsWith(Segment segment) => segment.Start <= End && segment.End >= Start;

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
