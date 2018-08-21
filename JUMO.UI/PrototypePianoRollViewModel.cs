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
    class PrototypePianoRollViewModel : INotifyPropertyChanged
    {
        private int _zoomFactor = 24;
        private int _gridUnit = 16;

        public int Numerator => Song.Current.Numerator;
        public int Denominator => Song.Current.Denominator;
        public int TimeResolution => Song.Current.TimeResolution;

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

        public ObservableCollection<Note> Notes { get; } = new ObservableCollection<Note>()
        {
            new Note(60, 64, 0, 480),
            new Note(62, 80, 480, 480),
            new Note(64, 96, 960, 480),
            new Note(65, 112, 1440, 480),
            new Note(67, 127, 1920, 1920),
            new Note(120, 60, 0, 480),
            new Note(122, 48, 480, 480),
            new Note(124, 36, 960, 480),
            new Note(125, 24, 1440, 480),
            new Note(127, 12, 1920, 1920)
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public PrototypePianoRollViewModel()
        {
            Song.Current.PropertyChanged += CurrentSong_PropertyChanged;
        }

        private void CurrentSong_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => OnPropertyChanged(e.PropertyName);

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
