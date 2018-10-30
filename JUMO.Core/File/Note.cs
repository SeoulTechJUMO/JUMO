using System;

namespace JUMO.File
{
    [Serializable]
    class Note
    {
        public int Start { get; }
        public int Length { get; }
        public byte Value { get; }
        public byte Velocity { get; }

        public Note(JUMO.Note source)
        {
            Start = source.Start;
            Length = source.Length;
            Value = source.Value;
            Velocity = source.Velocity;
        }
    }
}
