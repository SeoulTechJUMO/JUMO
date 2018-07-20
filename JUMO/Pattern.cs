using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO
{
    class Pattern
    {
        public string Name { get; set; }
        public IEnumerable<Score> Scores { get; set; }
        public IEnumerable<VstChannel> Channels { get; }
    }
}
