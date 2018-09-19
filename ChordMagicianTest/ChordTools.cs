using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordMagicianTest
{
    public class ChordTools
    {
        public static string GetChordName(string id, string key, string mode)
        {
            string ChordName = "";

            // 계산할 스케일 
            List<byte> Scale = Naming.CalScale(key, Naming.Scale[mode]);

            try
            {
                ChordName = Naming.id2chord[id];
            }
            catch
            {
                ChordName = "";
            }
                

            return ChordName;
        }
    }
}
