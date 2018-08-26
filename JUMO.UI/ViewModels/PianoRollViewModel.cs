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
    public class PianoRollViewModel : WorkspaceViewModel
    {
        private const int ZOOM_INIT = 24;
        private const int ZOOM_MIN = 1;
        private const int ZOOM_MAX = 96;

        private int _zoomFactor = 24;
        private int _gridUnit = 16;

        public override WorkspaceKey Key { get; }
        public override string DisplayName => $"피아노 롤: {Plugin.Name}";

        public Vst.Plugin Plugin { get; }
        public Pattern Pattern => Song.Current.CurrentPattern;

        public int Numerator => Song.Current.Numerator;
        public int Denominator => Song.Current.Denominator;
        public int TimeResolution => Song.Current.TimeResolution;

        public int ZoomFactor
        {
            get => _zoomFactor;
            set
            {
                _zoomFactor = Math.Max(ZOOM_MIN, Math.Min(value, ZOOM_MAX));
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

        public PianoRollViewModel(Vst.Plugin plugin)
        {
            Plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            Key = new PianoRollWorkspaceKey(plugin);
            Song.Current.PropertyChanged += CurrentSong_PropertyChanged;
        }

        private void CurrentSong_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);

            if (e.PropertyName.Equals(nameof(Song.CurrentPattern)))
            {
                OnPropertyChanged(nameof(Notes));
            }
        }
    }
}
