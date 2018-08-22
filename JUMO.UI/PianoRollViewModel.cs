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
    class PianoRollViewModel : INotifyPropertyChanged
    {
        private int _zoomFactor = 24;
        private int _gridUnit = 16;

        public Vst.Plugin Plugin { get; }
        public Pattern Pattern { get; }

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

        public ObservableCollection<Note> Notes => Pattern[Plugin];

        public event PropertyChangedEventHandler PropertyChanged;

        public PianoRollViewModel(Vst.Plugin plugin, Pattern pattern)
        {
            Plugin = plugin;
            Pattern = pattern;
            Song.Current.PropertyChanged += CurrentSong_PropertyChanged;
        }

        private void CurrentSong_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => OnPropertyChanged(e.PropertyName);

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
