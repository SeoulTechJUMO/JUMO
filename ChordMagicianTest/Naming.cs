using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordMagicianTest
{
    public static class Naming
    {
        public static readonly Dictionary<string, byte> NoteName = new Dictionary<string, byte>()
        {
            {"C",0 }, {"C#",1 }, {"D",2 }, {"D#",3 }, {"E",4 }, {"F",5 }, {"F#",6 }, {"G",7 }, {"G#",8 }, {"A",9 }, {"A#",10 }, {"B",11 },
        };

        public static readonly List<byte> Major = new List<byte>()
        {
            0,2,4,5,7,9,11
        };

        public static readonly List<byte> Minor = new List<byte>()
        {
            0,2,3,5,7,9,10
        };

        public static readonly Dictionary<string, List<byte>> Scale = new Dictionary<string, List<byte>>()
        {
            {"Major", Major }, {"Minor", Minor }
        };

        public static List<byte> CalScale(string key, List<byte> scale)
        {
            List<byte> _scale = new List<byte>();
            foreach (byte i in scale)
            {
                _scale.Add((byte)(i+NoteName[key]));
            }
            return _scale;
        }
    }
}
