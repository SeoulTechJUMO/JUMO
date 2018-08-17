using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO
{
    public class Track
    {
        public string Name { get; set; }
        public IEnumerable<PatternPlacement> Patterns { get; } = new List<PatternPlacement>();

        public Track(string name) => Name = name;
    }
}
