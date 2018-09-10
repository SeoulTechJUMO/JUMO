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

        protected override double ZoomBase => 4.0;

        public override WorkspaceKey Key => PlaylistWorkspaceKey.Instance;
        public override string DisplayName { get; } = "플레이리스트";

        public override IEnumerable<int> GridStepOptions { get; } = new[] { 1, 2, 4 };

        public ObservableCollection<PatternPlacement> PlacedPatterns { get; } = new ObservableCollection<PatternPlacement>();

        public PlaylistViewModel()
        {
            for (int i = 0; i < Song.NumOfTracks; i++)
            {
                _tracks[i].CollectionChanged += OnTrackCollectionChanged;

                foreach (PatternPlacement pp in _tracks[i])
                {
                    PlacedPatterns.Add(pp);
                }
            }
        }

        public void PlacePattern(Pattern pattern, int trackIndex, long start) => _tracks[trackIndex].PlacePattern(pattern, start);

        public void RemovePattern(PatternPlacement patternPlacement) => _tracks[patternPlacement.TrackIndex].Remove(patternPlacement);

        private void OnTrackCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (PatternPlacement pp in e.OldItems)
                {
                    PlacedPatterns.Remove(pp);
                }
            }

            if (e.NewItems != null)
            {
                foreach (PatternPlacement pp in e.NewItems)
                {
                    PlacedPatterns.Add(pp);
                }
            }
        }
    }
}
