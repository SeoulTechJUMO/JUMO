using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    public class PlaylistViewModel : MusicalCanvasWorkspaceViewModel
    {
        private readonly Track[] _tracks = Song.Current.Tracks;
        private readonly Dictionary<PatternPlacement, PatternPlacementViewModel> _vmTable = new Dictionary<PatternPlacement, PatternPlacementViewModel>();

        protected override double ZoomBase => 4.0;

        public override WorkspaceKey Key => PlaylistWorkspaceKey.Instance;
        public override string DisplayName { get; } = "플레이리스트";

        public override IEnumerable<int> GridStepOptions { get; } = new[] { 1, 2, 4 };

        public ObservableCollection<PatternPlacementViewModel> PlacedPatterns { get; } = new ObservableCollection<PatternPlacementViewModel>();

        public PlaylistViewModel()
        {
            for (int i = 0; i < Song.NumOfTracks; i++)
            {
                _tracks[i].CollectionChanged += OnTrackCollectionChanged;

                foreach (PatternPlacement pp in _tracks[i])
                {
                    PlacePatternInternal(pp);
                }
            }
        }

        public void PlacePattern(Pattern pattern, int trackIndex, long start) => PatternPlacement.Create(pattern, trackIndex, start);
        public void RemovePattern(PatternPlacement pp) => _tracks[pp.TrackIndex].Remove(pp);

        private void PlacePatternInternal(PatternPlacement pp)
        {
            PatternPlacementViewModel vm = new PatternPlacementViewModel(pp);

            _vmTable.Add(pp, vm);
            PlacedPatterns.Add(vm);
        }

        private void RemovePatternInternal(PatternPlacement pp)
        {
            if (_vmTable.TryGetValue(pp, out PatternPlacementViewModel vm))
            {
                PlacedPatterns.Remove(vm);
                _vmTable.Remove(pp);
            }
        }

        private void OnTrackCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (PatternPlacement pp in e.OldItems)
                {
                    RemovePatternInternal(pp);
                }
            }

            if (e.NewItems != null)
            {
                foreach (PatternPlacement pp in e.NewItems)
                {
                    PlacePatternInternal(pp);
                }
            }
        }
    }
}
