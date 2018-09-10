using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO
{
    /// <summary>
    /// 트랙 상에 배치된 패턴을 나타냅니다.
    /// </summary>
    public class PatternPlacement : INotifyPropertyChanged
    {
        private int _trackIndex;
        private long _start;

        /// <summary>
        /// 배치된 패턴 인스턴스를 가져옵니다.
        /// </summary>
        public Pattern Pattern { get; }

        public int TrackIndex
        {
            get => _trackIndex;
            internal set
            {
                if (_trackIndex != value)
                {
                    if (_trackIndex < 0 || _trackIndex >= Song.NumOfTracks)
                    {
                        throw new IndexOutOfRangeException($"{nameof(TrackIndex)} must be in [0, {Song.NumOfTracks})");
                    }

                    _trackIndex = value;
                    OnPropertyChanged(nameof(TrackIndex));
                }
            }
        }

        /// <summary>
        /// 배치된 패턴의 시작 시점을 가져오거나 설정합니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Start
        {
            get => _start;
            set
            {
                if (_start != value)
                {
                    _start = value;
                    OnPropertyChanged(nameof(Start));
                }
            }
        }

        /// <summary>
        /// 배치된 패턴의 길이를 가져옵니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Length => Pattern.Length;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 새로운 PatternPlacement 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="pattern">배치된 패턴</param>
        /// <param name="trackIndex">패턴이 배치된 트랙의 인덱스</param>
        /// <param name="start">배치된 패턴의 시작 지점 (PPQN 기반)</param>
        internal PatternPlacement(Pattern pattern, int trackIndex, long start)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            TrackIndex = trackIndex;
            Start = start;

            pattern.PropertyChanged += OnPatternPropertyChanged;
        }

        private void OnPatternPropertyChanged(object sender, PropertyChangedEventArgs e)
            => OnPropertyChanged(e.PropertyName);

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
