using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUMO.Vst;

namespace JUMO
{
    /// <summary>
    /// 음악을 구성하는 각각의 패턴을 나타냅니다.
    /// </summary>
    public class Pattern : INotifyPropertyChanged
    {
        private string _name;
        private long _length;

        private IDictionary<Plugin, ObservableCollection<Note>> _scores { get; } = new Dictionary<Plugin, ObservableCollection<Note>>();

        /// <summary>
        /// 패턴의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// 패턴의 길이를 가져오거나 설정합니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Length
        {
            get => _length;
            set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        /// <summary>
        /// 인덱스로 지정된 VST 플러그인에 대응하는 악보를 나타내는 컬렉션을 가져옵니다.
        /// </summary>
        /// <param name="p">VST 플러그인</param>
        public ObservableCollection<Note> this[Plugin p]
        {
            get
            {
                if (_scores.TryGetValue(p, out ObservableCollection<Note> score))
                {
                    return score;
                }
                else
                {
                    var newScore = new ObservableCollection<Note>();
                    _scores.Add(p, newScore);

                    return newScore;
                }
            }
        }

        /// <summary>
        /// 새로운 Pattern 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="name">패턴의 이름</param>
        public Pattern(string name) => Name = name;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
