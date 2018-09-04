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
        private const double ZOOM_BASE = 24.0;
        private const int ZOOM_PERCENT_MIN = 25;
        private const int ZOOM_PERCENT_MAX = 400;

        private int _zoomPercent = 100;
        private int _gridUnit = 16;

        private RelayCommand _zoomInCommand;
        private RelayCommand _zoomOutCommand;
        private RelayCommand _resetZoomCommand;

        public override WorkspaceKey Key { get; }
        public override string DisplayName => $"피아노 롤: {Plugin.Name}";

        public Vst.Plugin Plugin { get; }
        public Pattern Pattern => Song.Current.CurrentPattern;

        public int Numerator => Song.Current.Numerator;
        public int Denominator => Song.Current.Denominator;
        public int TimeResolution => Song.Current.TimeResolution;

        public int ZoomFactor { get; private set; } = 24;

        public int ZoomPercent
        {
            get => _zoomPercent;
            private set
            {
                _zoomPercent = Math.Max(ZOOM_PERCENT_MIN, Math.Min(value, ZOOM_PERCENT_MAX));
                ZoomFactor = (int)(ZOOM_BASE * _zoomPercent / 100.0);

                OnPropertyChanged(nameof(ZoomPercent));
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

        public ObservableCollection<Note> SelectedNotes { get; } = new ObservableCollection<Note>();

        public RelayCommand ZoomInCommand
        {
            get => _zoomInCommand ?? (_zoomInCommand = new RelayCommand(
                    _ => ZoomPercent += ZoomPercent >= 100 ? 10 : 5,
                    _ => ZoomPercent < ZOOM_PERCENT_MAX
                ));
        }

        public RelayCommand ZoomOutCommand
        {
            get => _zoomOutCommand ?? (_zoomOutCommand = new RelayCommand(
                    _ => ZoomPercent -= ZoomPercent > 100 ? 10 : 5,
                    _ => ZoomPercent > ZOOM_PERCENT_MIN
                ));
        }

        public RelayCommand ResetZoomCommand
        {
            get => _resetZoomCommand ?? (_resetZoomCommand = new RelayCommand(
                    _ => ZoomPercent = 100,
                    _ => ZoomPercent != 100
                ));
        }

        public PianoRollViewModel(Vst.Plugin plugin)
        {
            Plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            Key = new PianoRollWorkspaceKey(plugin);
            Song.Current.PropertyChanged += CurrentSong_PropertyChanged;
        }

        public void SelectItems(IEnumerable items)
        {
            if (items != null)
            {
                foreach (var item in items.OfType<Note>())
                {
                    SelectedNotes.Add(item);
                }
            }
        }

        public void DeselectItems(IEnumerable items)
        {
            if (items != null)
            {
                foreach (var item in items.OfType<Note>())
                {
                    SelectedNotes.Remove(item);
                }
            }
        }

        public void ClearSelection()
        {
            SelectedNotes?.Clear();
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
