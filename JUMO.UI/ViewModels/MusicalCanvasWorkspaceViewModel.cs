using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JUMO.UI
{
    public abstract class MusicalCanvasWorkspaceViewModel : WorkspaceViewModel
    {
        protected abstract double ZoomBase { get; }
        protected virtual int ZoomPercentMinimum => 25;
        protected virtual int ZoomPercentMaximum => 400;

        private int _zoomPercent = 100;
        private int _gridStep = 1;
        private bool _snapToGrid = true;

        private RelayCommand _zoomInCommand;
        private RelayCommand _zoomOutCommand;
        private RelayCommand _resetZoomCommand;

        public Song Song => Song.Current;
        public Playback.MasterSequencer Sequencer => Playback.MasterSequencer.Instance;

        public double ZoomFactor { get; private set; }

        public int ZoomPercent
        {
            get => _zoomPercent;
            private set
            {
                _zoomPercent = Math.Max(ZoomPercentMinimum, Math.Min(value, ZoomPercentMaximum));
                ZoomFactor = ZoomBase * _zoomPercent / 100.0;

                OnPropertyChanged(nameof(ZoomPercent));
                OnPropertyChanged(nameof(ZoomFactor));
            }
        }

        public abstract IEnumerable<int> GridStepOptions { get; }

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

        public ObservableCollection<IMusicalItem> SelectedItems { get; } = new ObservableCollection<IMusicalItem>();

        public RelayCommand ZoomInCommand
        {
            get => _zoomInCommand ?? (_zoomInCommand = new RelayCommand(
                    _ => ZoomPercent += ZoomPercent >= 100 ? 10 : 5,
                    _ => ZoomPercent < ZoomPercentMaximum
                ));
        }

        public RelayCommand ZoomOutCommand
        {
            get => _zoomOutCommand ?? (_zoomOutCommand = new RelayCommand(
                    _ => ZoomPercent -= ZoomPercent > 100 ? 10 : 5,
                    _ => ZoomPercent > ZoomPercentMinimum
                ));
        }

        public RelayCommand ResetZoomCommand
        {
            get => _resetZoomCommand ?? (_resetZoomCommand = new RelayCommand(
                    _ => ZoomPercent = 100,
                    _ => ZoomPercent != 100
                ));
        }

        public abstract RelayCommand CutCommand { get; }

        public abstract RelayCommand CopyCommand { get; }

        public abstract RelayCommand PasteCommand { get; }

        public abstract RelayCommand DeleteCommand { get; }

        protected MusicalCanvasWorkspaceViewModel()
        {
            ZoomFactor = ZoomBase;
        }

        public void SelectItems(IEnumerable items)
        {
            if (items != null)
            {
                foreach (var item in items.OfType<IMusicalItem>())
                {
                    SelectedItems.Add(item);
                }
            }
        }

        public void DeselectItems(IEnumerable items)
        {
            if (items != null)
            {
                foreach (var item in items.OfType<IMusicalItem>())
                {
                    SelectedItems.Remove(item);
                }
            }
        }

        public void ClearSelection()
        {
            SelectedItems?.Clear();
        }
    }
}
