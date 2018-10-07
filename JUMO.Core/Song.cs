﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace JUMO
{
    public sealed class Song : INotifyPropertyChanged, IDisposable
    {
        #region Singleton

        private static readonly Lazy<Song> _current = new Lazy<Song>(() => new Song());

        private Song()
        {
            SetTempo(6, 120);

            for (int i = 0; i < NumOfTracks; i++)
            {
                Tracks[i] = new Track(this, i, $"트랙 {i + 1}");
            }

            for (int i = 0; i < 16; i++)
            {
                Patterns.Add(new Pattern(this, $"패턴 {i + 1}"));
            }

            Sequencer = new Playback.MasterSequencer(this);
            CurrentPattern = Patterns[0];

            PlacedPatterns.CollectionChanged += OnPlacedPatternsCollectionChanged;

            UpdateLength();
        }

        /// <summary>
        /// 현재 로드된 곡에 대한 Song 인스턴스를 가져옵니다.
        /// </summary>
        public static Song Current => _current.Value;

        #endregion

        /// <summary>
        /// 트랙의 총 개수를 정의하는 상수입니다.
        /// </summary>
        public const int NumOfTracks = 64;

        private string _title = "제목 없음";
        private string _artist = "";
        private string _genre = "";
        private string _description = "";
        private int _numerator = 4;
        private int _denominator = 4;
        private int _timeResolution = 96;
        private long _length = 0;
        private Pattern _currentPattern;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 음악 프로젝트의 제목을 가져오거나 설정합니다.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        /// <summary>
        /// 작곡가의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Artist
        {
            get => _artist;
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        /// <summary>
        /// 장르 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Genre
        {
            get => _genre;
            set
            {
                _genre = value;
                OnPropertyChanged(nameof(Genre));
            }
        }

        /// <summary>
        /// 프로젝트 설명 내용을 가져오거나 설정합니다.
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        /// <summary>
        /// 제목, 작곡가, 장르, 설명 외에 추가적인 정보를 저장할 수 있는 이름-값 쌍의 컬렉션을 가져옵니다.
        /// </summary>
        public NameValueCollection MiscMetadata { get; } = new NameValueCollection() { };

        /// <summary>
        /// 곡의 템포를 가져옵니다.
        /// </summary>
        public double Tempo { get; private set; }

        /// <summary>
        /// 템포의 기준이 되는 박자의 길이를 가져옵니다.
        /// 4분음표 네 개의 길이는 24로 정의합니다.
        /// </summary>
        public int TempoBeat { get; private set; }

        /// <summary>
        /// 곡의 템포를 MIDI 템포 형식으로 가져오거나 설정합니다.
        /// 4분음표 하나가 연주되는 시간을 마이크로초 단위로 나타낸 값입니다.
        /// </summary>
        public int MidiTempo { get; private set; }

        /// <summary>
        /// 곡의 박자표 중 분자를 가져오거나 설정합니다.
        /// </summary>
        public int Numerator
        {
            get => _numerator;
            set
            {
                _numerator = value;
                OnPropertyChanged(nameof(Numerator));
            }
        }

        /// <summary>
        /// 곡의 박자표 중 분모를 가져오거나 설정합니다.
        /// </summary>
        public int Denominator
        {
            get => _denominator;
            set
            {
                _denominator = value;
                OnPropertyChanged(nameof(Denominator));
            }
        }

        /// <summary>
        /// PPQN (Pulses Per Quarter Note) 값을 가져오거나 설정합니다.
        /// </summary>
        public int TimeResolution
        {
            get => _timeResolution;
            set
            {
                _timeResolution = value;
                OnPropertyChanged(nameof(TimeResolution));
            }
        }

        /// <summary>
        /// 곡의 총 길이를 가져옵니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Length
        {
            get => _length;
            private set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        /// <summary>
        /// 곡을 구성하는 트랙의 배열을 가져옵니다.
        /// </summary>
        public Track[] Tracks { get; } = new Track[NumOfTracks];

        /// <summary>
        /// 곡을 구성하는 패턴을 저장하는 컬렉션을 가져옵니다.
        /// </summary>
        public ObservableCollection<Pattern> Patterns { get; } = new ObservableCollection<Pattern>();

        /// <summary>
        /// 플레이리스트 상에 배치되어 있는 모든 패턴을 저장하는 컬렉션을 가져옵니다.
        /// </summary>
        public ObservableCollection<PatternPlacement> PlacedPatterns { get; } = new ObservableCollection<PatternPlacement>();

        /// <summary>
        /// 현재 선택된 패턴을 가져오거나 설정합니다.
        /// </summary>
        public Pattern CurrentPattern
        {
            get => _currentPattern;
            set
            {
                _currentPattern = value;
                OnPropertyChanged(nameof(CurrentPattern));
            }
        }

        /// <summary>
        /// 이 Song을 재생할 수 있는 시퀀서를 가져옵니다.
        /// </summary>
        public Playback.MasterSequencer Sequencer { get; }

        /// <summary>
        /// 곡의 템포를 설정합니다.
        /// </summary>
        /// <param name="tempoBeat">템포의 기준이 되는 박자의 길이</param>
        /// <param name="tempo">템포 값</param>
        public void SetTempo(int tempoBeat, double tempo)
        {
            TempoBeat = tempoBeat;
            Tempo = tempo;
            MidiTempo = (int)Math.Round(10_000_000.0 * TempoBeat / Tempo);

            OnPropertyChanged(nameof(Tempo));
            OnPropertyChanged(nameof(TempoBeat));
            OnPropertyChanged(nameof(MidiTempo));
        }

        private void UpdateLength() => Length = PlacedPatterns.Select(pp => pp.Start + pp.Length).DefaultIfEmpty(0).Max();

        private void OnPlacedPatternsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateLength();
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Dispose()
        {
            Sequencer.Dispose();
        }
    }
}
