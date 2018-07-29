using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JUMO.UI
{
    class PrototypePianoRollViewModel : IPianoRollViewModel, INotifyPropertyChanged
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
                OnPropertyChanged(nameof(Numerator));
            }
        }

        public int Denominator
        {
            get => _denominator;
            set
            {
                System.Diagnostics.Debug.WriteLine($"Setting {nameof(Denominator)} to {value}");
                _denominator = value;
                OnPropertyChanged(nameof(Denominator));
            }
        }

        public int TimeResolution => 480;

        public int ZoomFactor
        {
            get => _zoomFactor;
            set
            {
                System.Diagnostics.Debug.WriteLine($"Setting {nameof(ZoomFactor)} to {value}");
                _zoomFactor = value;
                OnPropertyChanged(nameof(ZoomFactor));
            }
        }

        public int GridUnit
        {
            get => _gridUnit;
            set
            {
                System.Diagnostics.Debug.WriteLine($"Setting {nameof(GridUnit)} to {value}");
                _gridUnit = value;
                OnPropertyChanged(nameof(GridUnit));
            }
        }

        public ObservableCollection<INote> Notes { get; } = new ObservableCollection<INote>()
        {
            new PrototypeNote(60, 64, 0, 480),
            new PrototypeNote(62, 64, 480, 480),
            new PrototypeNote(64, 64, 960, 480),
            new PrototypeNote(65, 64, 1440, 480),
            new PrototypeNote(67, 64, 1920, 1920),
            new PrototypeNote(120, 64, 0, 480),
            new PrototypeNote(122, 64, 480, 480),
            new PrototypeNote(124, 64, 960, 480),
            new PrototypeNote(125, 64, 1440, 480),
            new PrototypeNote(127, 64, 1920, 1920)
        };

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
