using System;

namespace JUMO.File
{
    [Serializable]
    class Song
    {
        // From Song
        public string Title { get; }
        public string Artist { get; }
        public string Genre { get; }
        public string Description { get; }
        public double Tempo { get; }
        public int TempoBeat { get; }
        public int Numerator { get; }
        public int Denominator { get; }
        public int TimeResolution { get; }

        public Song(JUMO.Song source)
        {
            Title = source.Title;
            Artist = source.Artist;
            Genre = source.Genre;
            Description = source.Description;
            Tempo = source.Tempo;
            TempoBeat = source.TempoBeat;
            Numerator = source.Numerator;
            Denominator = source.Denominator;
            TimeResolution = source.TimeResolution;
        }
    }
}
