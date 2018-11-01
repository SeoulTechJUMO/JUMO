using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace JUMO.UI
{
    public class PlaylistViewModel : MusicalCanvasWorkspaceViewModel
    {
        private readonly ObservableCollection<PatternPlacement> _placedPatterns = Song.Current.PlacedPatterns;
        private readonly Dictionary<PatternPlacement, PatternPlacementViewModel> _vmTable = new Dictionary<PatternPlacement, PatternPlacementViewModel>();

        protected override double ZoomBase => 4.0;

        public override WorkspaceKey Key => PlaylistWorkspaceKey.Instance;

        public override string DisplayName { get; } = "플레이리스트";

        public override IEnumerable<int> GridStepOptions { get; } = new[] { 1, 2, 4 };

        public IEnumerable<Track> Tracks => Song.Tracks;

        public ObservableCollection<PatternPlacementViewModel> PlacedPatterns { get; } = new ObservableCollection<PatternPlacementViewModel>();

        public PlaylistViewModel()
        {
            _placedPatterns.CollectionChanged += OnPlacedPatternsCollectionChanged;

            foreach (PatternPlacement pp in _placedPatterns)
            {
                PlacePatternInternal(pp);
            }
        }

        public void PlacePattern(Pattern pattern, int trackIndex, int start) => _placedPatterns.Add(new PatternPlacement(pattern, trackIndex, start));

        public void RemovePattern(PatternPlacement pp) => _placedPatterns.Remove(pp);

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

        private void OnPlacedPatternsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _vmTable.Clear();
                PlacedPatterns.Clear();
            }
        }
    }
}
