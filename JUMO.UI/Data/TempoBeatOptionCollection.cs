using System.Collections.Generic;
using System.Windows.Media;

namespace JUMO.UI.Data
{
    class TempoBeatOption
    {
        public int Value { get; set; }
        public Geometry Icon { get; set; }
    }

    class TempoBeatOptionCollection : List<TempoBeatOption> { }
}
