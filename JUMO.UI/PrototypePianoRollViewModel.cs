using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    class PrototypePianoRollViewModel
    {
        private int _numerator = 4;
        private int _denominator = 4;
        private int _zoomFactor = 24;
        private int _gridUnit = 16;

        public int Numerator
        {
            get => _numerator;
            set
            {
                System.Diagnostics.Debug.WriteLine($"Setting {nameof(Numerator)} to {value}");
                _numerator = value;
            }
        }

        public int Denominator
        {
            get => _denominator;
            set
            {
                System.Diagnostics.Debug.WriteLine($"Setting {nameof(Denominator)} to {value}");
                _denominator = value;
            }
        }

        public int ZoomFactor
        {
            get => _zoomFactor;
            set
            {
                System.Diagnostics.Debug.WriteLine($"Setting {nameof(ZoomFactor)} to {value}");
                _zoomFactor = value;
            }
        }

        public int GridUnit
        {
            get => _gridUnit;
            set
            {
                System.Diagnostics.Debug.WriteLine($"Setting {nameof(GridUnit)} to {value}");
                _gridUnit = value;
            }
        }
    }
}
