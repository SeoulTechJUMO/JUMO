using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using JUMO.Vst;

namespace JUMO
{
    /// <summary>
    /// 음악을 구성하는 각각의 패턴을 나타냅니다.
    /// </summary>
    public class Pattern : INotifyPropertyChanged
    {
        private readonly Dictionary<Plugin, Score> _scores = new Dictionary<Plugin, Score>();
        private readonly Song _song;
        private string _name;
        private int _length;

        public event EventHandler<ScoreChangedEventArgs> ScoreChanged;

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
        public int Length
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
        /// 현재 사용할 수 있는 Score 인스턴스들을 반환하는 반복기를 가져옵니다.
        /// </summary>
        public IEnumerable<Score> Scores
        {
            get
            {
                foreach (Score score in _scores.Values)
                {
                    yield return score;
                }
            }
        }

        /// <summary>
        /// 인덱스로 지정된 VST 플러그인에 대응하는 악보를 나타내는 컬렉션을 가져옵니다.
        /// </summary>
        /// <param name="p">VST 플러그인</param>
        public Score this[Plugin p]
        {
            get
            {
                if (_scores.TryGetValue(p, out Score score))
                {
                    return score;
                }
                else
                {
                    var newScore = new Score(this);
                    ((INotifyPropertyChanged)newScore).PropertyChanged += OnScorePropertyChanged;

                    _scores.Add(p, newScore);
                    ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(new[] { newScore }, null));

                    return newScore;
                }
            }
        }

        /// <summary>
        /// 새로운 Pattern 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="owner">이 패턴을 소유하는 Song 인스턴스</param>
        /// <param name="name">패턴의 이름</param>
        internal Pattern(Song owner, string name)
        {
            _song = owner;
            _song.PropertyChanged += OnSongPropertyChanged;
            Name = name;

            PluginManager.Instance.Plugins.CollectionChanged += OnPluginsCollectionChanged;

            UpdateLength();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnSongPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Song.Numerator):
                case nameof(Song.Denominator):
                case nameof(Song.TimeResolution):
                    UpdateLength();
                    break;
            }
        }

        private void OnScorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Score.Length))
            {
                UpdateLength();
            }
        }

        private void UpdateLength()
        {
            int ticksPerBar = 4 * _song.TimeResolution * _song.Numerator / _song.Denominator;
            int maxLength = Math.Max(1, _scores.Values.Select(score => score.Length).DefaultIfEmpty(0).Max());
            int q = Math.DivRem(maxLength, ticksPerBar, out int r);

            Length = (q + (r == 0 ? 0 : 1)) * ticksPerBar;
        }

        private void OnPluginsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (Plugin plugin in e.OldItems)
                {
                    if (_scores.TryGetValue(plugin, out Score score))
                    {
                        _scores.Remove(plugin);
                        ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(null, new[] { score }));
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems?[0] is Plugin oldPlugin && e.NewItems?[0] is Plugin newPlugin)
                {
                    if (_scores.TryGetValue(oldPlugin, out Score score))
                    {
                        _scores.Remove(oldPlugin);
                        _scores.Add(newPlugin, score);
                    }
                }
            }

            UpdateLength();
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class ScoreChangedEventArgs : EventArgs
    {
        public IList<Score> CreatedScores { get; }
        public IList<Score> RemovedScores { get; }

        public ScoreChangedEventArgs(IList<Score> createdScores, IList<Score> removedScores)
        {
            CreatedScores = createdScores;
            RemovedScores = removedScores;
        }
    }
}
