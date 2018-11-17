using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace JUMO
{
    /// <summary>
    /// 패턴을 배치할 수 있는 트랙을 나타냅니다.
    /// </summary>
    public class Track : List<PatternPlacement>, INotifyPropertyChanged
    {
        private readonly Song _song;
        private string _name;

        #region Properties

        /// <summary>
        /// 트랙의 인덱스 번호를 가져옵니다.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 트랙의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        #endregion

        /// <summary>
        /// 새로운 Track 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="song">새 트랙을 소유하는 Song 인스턴스</param>
        /// <param name="index">새 트랙의 인덱스 번호</param>
        /// <param name="name">새 트랙의 이름</param>
        public Track(Song song, int index, string name)
        {
            _song = song ?? throw new ArgumentNullException(nameof(song));
            Index = index;
            Name = name;
        }

        internal IEnumerable<int> GetTickIterator(Playback.MasterSequencer masterSequener, int startPosition)
        {
            IEnumerator<PatternPlacement> enumerator = this.OrderBy(pp => pp.Start).GetEnumerator();

            bool hasNext;
            PatternPlacement previous = null;

            for (hasNext = enumerator.MoveNext();
                 hasNext && enumerator.Current.Start < startPosition;
                 previous = enumerator.Current, hasNext = enumerator.MoveNext()) ;

            int ticks = startPosition;

            if (previous != null && previous.Start + previous.Length >= ticks)
            {
                masterSequener.PlayPattern(previous.Pattern, ticks - previous.Start);
            }

            while (hasNext)
            {
                while (ticks < enumerator.Current.Start)
                {
                    yield return ticks;

                    ticks++;
                }

                yield return ticks;

                while (hasNext && enumerator.Current.Start == ticks)
                {
                    System.Diagnostics.Debug.WriteLine($"[{ticks,-8}] Track({Name}): 이번에 재생할 패턴은 '{enumerator.Current.Pattern.Name}");
                    masterSequener.PlayPattern(enumerator.Current.Pattern, 0);

                    hasNext = enumerator.MoveNext();
                }

                ticks++;
            }

            if (enumerator.Current != null)
            {
                int lastPatternEnd = enumerator.Current.Start + enumerator.Current.Length;

                while (ticks < lastPatternEnd)
                {
                    yield return ticks;

                    ticks++;
                }
            }

            masterSequener.HandleFinishedTrack();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
