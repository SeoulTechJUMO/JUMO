using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    class PrototypeNote : INote
    {
        public int Value { get; set; }

        public int Velocity { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public PrototypeNote(int value, int velocity, int start, int length)
        {
            Value = value;
            Velocity = velocity;
            Start = start;
            Length = length;
        }
    }
}
