using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO
{
    public sealed class Song
    {
        #region Singleton

        private static readonly Lazy<Song> _current = new Lazy<Song>(() => new Song());

        private Song()
        {
            Tempo = 120;

            for (int i = 0; i < NumOfTracks; i++)
            {
                Tracks[i] = new Track($"트랙 {i + 1}");
            }

            Patterns.Add(new Pattern("패턴 1"));
        }

        /// <summary>
        /// 현재 로드된 곡에 대한 Song 인스턴스를 가져옵니다.
        /// </summary>
        public static Song Current => _current.Value;

        #endregion

        private const int NumOfTracks = 64;

        private int _tempo;

        /// <summary>
        /// 음악 프로젝트의 제목을 가져오거나 설정합니다.
        /// </summary>
        public string Title { get; set; } = "제목 없음";

        /// <summary>
        /// 작곡가의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Artist { get; set; } = "";

        /// <summary>
        /// 장르 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Genre { get; set; } = "";

        /// <summary>
        /// 프로젝트 설명 내용을 가져오거나 설정합니다.
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// 제목, 작곡가, 장르, 설명 외에 추가적인 정보를 저장할 수 있는 이름-값 쌍의 컬렉션을 가져옵니다.
        /// </summary>
        public NameValueCollection MiscMetadata { get; } = new NameValueCollection() { };

        /// <summary>
        /// 곡의 템포를 BPM 단위로 가져오거나 설정합니다.
        /// </summary>
        public int Tempo
        {
            get => _tempo;
            set
            {
                _tempo = value;
                MidiTempo = (int)Math.Round(60_000_000.0 / _tempo);
            }
        }

        /// <summary>
        /// 곡의 박자표 중 분자를 가져오거나 설정합니다.
        /// </summary>
        public int Numerator { get; set; } = 4;

        /// <summary>
        /// 곡의 박자표 중 분모를 가져오거나 설정합니다.
        /// </summary>
        public int Denominator { get; set; } = 4;

        /// <summary>
        /// 곡을 구성하는 트랙의 배열을 가져옵니다.
        /// </summary>
        public Track[] Tracks { get; } = new Track[NumOfTracks];

        /// <summary>
        /// 곡을 구성하는 패턴을 저장하는 컬렉션을 가져옵니다.
        /// </summary>
        public ICollection<Pattern> Patterns { get; } = new List<Pattern>();

        /// <summary>
        /// 곡의 템포를 MIDI 템포 형식으로 가져오거나 설정합니다.
        /// 4분음표 하나가 연주되는 시간을 마이크로초 단위로 나타낸 값입니다.
        /// </summary>
        public int MidiTempo { get; private set; }
    }
}
