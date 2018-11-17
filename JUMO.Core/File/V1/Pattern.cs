using System;

namespace JUMO.File.V1
{
    [Serializable]
    class Pattern
    {
        public int Id { get; }

        public string Name { get; }

        public Pattern(int id, JUMO.Pattern source)
        {
            Id = id;
            Name = source.Name;
        }
    }

    [Serializable]
    class PatternPlacement
    {
        public int TrackId { get; }
        public int PatternId { get; }
        public int Start { get; }
        public int Length { get; }
        public bool UseAutoLength { get; }

        public PatternPlacement(int patternId, JUMO.PatternPlacement source)
        {
            TrackId = source.TrackIndex;
            PatternId = patternId;
            Start = source.Start;
            Length = source.Length;
            UseAutoLength = source.UseAutoLength;
        }
    }
}
