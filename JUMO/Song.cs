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
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public NameValueCollection MiscMetadata { get; set; }

        public int Tempo { get; set; } // In BPM
        public int Numerator { get; set; }
        public int Denominator { get; set; }

        public IEnumerable<Track> Tracks { get; }

        public int MidiTempo => (int)Math.Round(60_000_000.0 / Tempo);
    }
}
