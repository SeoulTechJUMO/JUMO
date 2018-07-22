using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO
{
    class Song
    {
        private const int NumOfTracks = 64;

        public string Title { get; set; } = "제목 없음";
        public string Artist { get; set; } = "";
        public string Genre { get; set; } = "";
        public string Description { get; set; } = "";
        public NameValueCollection MiscMetadata { get; set; } = new NameValueCollection() { };

        public int Tempo { get; set; } = 120;
        public int Numerator { get; set; } = 4;
        public int Denominator { get; set; } = 4;

        public Track[] Tracks { get; } = new Track[NumOfTracks];

        public int MidiTempo => (int)Math.Round(60_000_000.0 / Tempo);

        public Song()
        {
            for (int i = 0; i < NumOfTracks; i++)
            {
                Tracks[i] = new Track($"트랙 {i + 1}");
            }
        }
    }
}
