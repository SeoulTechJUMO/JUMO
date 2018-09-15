using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace JUMO
{
    /// <summary>
    /// 패턴을 배치할 수 있는 트랙을 나타냅니다.
    /// </summary>
    public class Track : ObservableCollection<PatternPlacement>
    {
        private string _name;
        private long _length;

        /// <summary>
        /// 트랙에 배치된 임의의 PatternPlacement의 속성이 변경되었을 때 발생하는 이벤트입니다.
        /// </summary>
        public event EventHandler PatternPlacementPropertyChanged;

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
                _name = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        /// <summary>
        /// 트랙의 총 길이를 가져옵니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Length
        {
            get => _length;
            private set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Length)));
                }
            }
        }

        /// <summary>
        /// 새로운 Track 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="index">새 트랙의 인덱스 번호</param>
        /// <param name="name">새 트랙의 이름</param>
        internal Track(int index, string name)
        {
            Index = index;
            Name = name;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (PatternPlacement pp in e.OldItems)
                {
                    pp.PropertyChanged -= OnPatternPlacementPropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (PatternPlacement pp in e.NewItems)
                {
                    pp.PropertyChanged += OnPatternPlacementPropertyChanged;
                }
            }

            UpdateLength();
            base.OnCollectionChanged(e);
        }

        internal new void Add(PatternPlacement item) => base.Add(item);

        private void UpdateLength() => Length = this.Select(pp => pp.Start + pp.Length).DefaultIfEmpty(0).Max();

        private void OnPatternPlacementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateLength();
            PatternPlacementPropertyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
