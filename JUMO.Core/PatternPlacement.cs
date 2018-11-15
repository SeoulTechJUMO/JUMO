using System;
using System.ComponentModel;

namespace JUMO
{
    /// <summary>
    /// 트랙 상에 배치된 패턴을 나타냅니다.
    /// </summary>
    public class PatternPlacement : IMusicalItem, INotifyPropertyChanged
    {
        private int _trackIndex = -1;
        private int _start;
        private int _length;
        private bool _useAutoLength = false;

        /// <summary>
        /// 배치된 패턴 인스턴스를 가져옵니다.
        /// </summary>
        public Pattern Pattern { get; }

        public int TrackIndex
        {
            get => _trackIndex;
            set
            {
                if (_trackIndex != value)
                {
                    if (value < 0 || value >= Song.NumOfTracks)
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
        public int Start
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
        /// 배치된 패턴의 길이를 가져오거나 설정합니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// 이 값을 수동으로 설정할 경우 UseAutoLength 속성이 자동으로 해제됩니다.
        /// </summary>
        public int Length
        {
            get => _length;
            set
            {
                if (_length != value)
                {
                    SetLength(value);
                    UseAutoLength = false;
                }
            }
        }

        /// <summary>
        /// 배치된 패턴의 길이를 원본 패턴의 길이로 자동 설정할 것인지 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool UseAutoLength
        {
            get => _useAutoLength;
            set
            {
                if (_useAutoLength != value)
                {
                    if (_useAutoLength = value)
                    {
                        Pattern.PropertyChanged += OnPatternPropertyChanged;
                        SetLength(Pattern.Length);
                    }
                    else
                    {
                        Pattern.PropertyChanged -= OnPatternPropertyChanged;
                    }

                    OnPropertyChanged(nameof(UseAutoLength));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 새로운 PatternPlacement 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="pattern">배치된 패턴</param>
        /// <param name="trackIndex">패턴이 배치된 트랙의 인덱스</param>
        /// <param name="start">배치된 패턴의 시작 지점 (PPQN 기반)</param>
        public PatternPlacement(Pattern pattern, int trackIndex, int start)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            TrackIndex = trackIndex;
            Start = start;
            UseAutoLength = true;
        }

        private void SetLength(int newLength)
        {
            _length = newLength;
            OnPropertyChanged(nameof(Length));
        }

        private void OnPatternPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (UseAutoLength && e.PropertyName == nameof(Pattern.Length))
            {
                SetLength(Pattern.Length);
            }
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
