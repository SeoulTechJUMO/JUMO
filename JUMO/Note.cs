using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO
{
    class Note
    {
        public byte Value { get; set; }
        public byte Velocity { get; set; }
        public long Start { get; set; }
        public long Length { get; set; }

        public Note(byte value, byte velocity, long start, long length)
        {
            Value = value;
            Velocity = velocity;
            Start = start;
            Length = length;
        }
    }
}
