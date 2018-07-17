using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    public interface IPianoRollViewModel
    {
        int Numerator { get; }
        int Denominator { get; }
        int TimeResolution { get; }

        int ZoomFactor { get; set; }
        int GridUnit { get; set; }
        IEnumerable<INote> Notes { get; }
    }
}
