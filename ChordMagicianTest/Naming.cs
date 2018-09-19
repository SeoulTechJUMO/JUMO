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
                if (i + NoteName[key] > 11)
                {
                    _scale.Add((byte)(i + NoteName[key] - 12));
                }
                else
                {
                    _scale.Add((byte)(i + NoteName[key]));
                }
            }
            return _scale;
        }

        public static readonly Dictionary<string, string> id2chord = new Dictionary<string, string>()
        {
            { "1","C" }, {"2","Dm" }, {"3","Em" }, {"4","F" }, {"5","G" }, {"6","Am" }, {"7","Bdim" },
            { "16","C/E" }, {"17","CM7" }, {"164","C/G" }, {"142","CM7/B" }, {"b1","Cm" }, {"27","Dm7" },
            {"26","Dm/F" }, {"242","Dm7/C" }, {"264","Dm/A" }, { "265","FM6"}, {"L27","D7" }, {"37","Em7" },
            {"364","Em/B" }, {"365","GM6" }, {"36","Em/G" }, {"b3","D#" }, {"47", "FM7" }, {"b4","Fm"}, {"464","F/C" },
            {"46","F/A" }, {"D3","D#" }, {"D47","F7" }, {"57","G7" }, {"56","G/B" }, {"5/5","D" }, {"57/5","D7" },
            {"564","G/D" }, {"57/4","C7" }, {"5/6","E" }, {"542","G7/F" }, {"57/2","A7" }, {"5/2","A" }, {"565/6","E7/G#" },
            {"b5","Gm" }, {"5/3","B" }, {"56/6","E/G#" }, {"67","Am7" }, {"665","CM6" }, {"664","Am/E" }, {"66","Am/C" },
            {"642","Am7/G" }, {"b7","Bm" }, {"77","Bm7b5" }, {"7/5","F#dim" }, {"57/6","E7"}, {"7/6","G#dim" }, {"b6","G#" },
            
        };
    }
}
