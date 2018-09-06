using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    public class PlaylistViewModel : WorkspaceViewModel
    {
        private const double ZOOM_BASE = 4.0;
        private const int ZOOM_PERCENT_MIN = 25;
        private const int ZOOM_PERCENT_MAX = 400;

        private int _zoomPercent = 100;
        private int _gridStep = 1;
        private bool _snapToGrid = true;

        private RelayCommand _zoomInCommand;
        private RelayCommand _zoomOutCommand;
        private RelayCommand _resetZoomCommand;

        public override WorkspaceKey Key => PlaylistWorkspaceKey.Instance;
        public override string DisplayName { get; } = "플레이리스트";

        public int Numerator => Song.Current.Numerator;
        public int Denominator => Song.Current.Denominator;
        public int TimeResolution => Song.Current.TimeResolution;

        public double ZoomFactor { get; set; } = 4.0;

        public int ZoomPercent
        {
            get => _zoomPercent;
            private set
            {
                _zoomPercent = Math.Max(ZOOM_PERCENT_MIN, Math.Min(value, ZOOM_PERCENT_MAX));
                ZoomFactor = ZOOM_BASE * _zoomPercent / 100.0;

                OnPropertyChanged(nameof(ZoomPercent));
                OnPropertyChanged(nameof(ZoomFactor));
            }
        }

        public IEnumerable<int> GridStepOptions { get; } = new[] { 1, 2, 4 };

        public int GridStep
        {
            get => _gridStep;
            set
            {
                _gridStep = value;
                OnPropertyChanged(nameof(GridStep));
            }
        }

        public bool SnapToGrid
        {
            get => _snapToGrid;
            set
            {
                _snapToGrid = value;
                OnPropertyChanged(nameof(SnapToGrid));
            }
        }

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
    }
}
