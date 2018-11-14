using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace JUMO.UI
{
    public class PlaylistViewModel : MusicalCanvasWorkspaceViewModel
    {
        private readonly ObservableCollection<PatternPlacement> _placedPatterns = Song.Current.PlacedPatterns;
        private readonly Dictionary<PatternPlacement, PatternPlacementViewModel> _vmTable = new Dictionary<PatternPlacement, PatternPlacementViewModel>();

        #region Properties

        protected override double ZoomBase => 4.0;

        public override WorkspaceKey Key => PlaylistWorkspaceKey.Instance;

        public override string DisplayName { get; } = "플레이리스트";

        public override IEnumerable<int> GridStepOptions { get; } = new[] { 1, 2, 4 };

        public IEnumerable<Track> Tracks => Song.Tracks;

        public ObservableCollection<PatternPlacementViewModel> PlacedPatterns { get; } = new ObservableCollection<PatternPlacementViewModel>();

        #endregion

        public PlaylistViewModel()
        {
            _placedPatterns.CollectionChanged += OnPlacedPatternsCollectionChanged;

            foreach (PatternPlacement pp in _placedPatterns)
            {
                PlacePatternInternal(pp);
            }
        }

        public void PlacePattern(Pattern pattern, int trackIndex, int start) => _placedPatterns.Add(new PatternPlacement(pattern, trackIndex, start));

        private void PlacePattern(PatternPlacement pp) => _placedPatterns.Add(pp);

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

        protected override void ExecuteCut()
        {
            ExecuteCopy();
            ExecuteDelete();
        }

        protected override void ExecuteCopy()
        {
            Storage.Instance.PutItems(typeof(PlaylistViewModel), SelectedItems);
        }

        protected override void ExecutePaste()
        {
            if (!Storage.Instance.CurrentType.Equals(typeof(PlaylistViewModel)))
            {
                return;
            }

            ClearSelection();

            int start = Sequencer.Position;
            int clipStart = Storage.Instance.CurrentClip.Min(ppVm => ppVm.Start);

            IEnumerable<PatternPlacement> patternsToPlace =
                from PatternPlacementViewModel ppVm in Storage.Instance.CurrentClip
                select new PatternPlacement(ppVm.Pattern, ppVm.TrackIndex, ppVm.Start - clipStart + start);

            foreach(PatternPlacement pp in patternsToPlace)
            {
                PlacePattern(pp);
                SelectedItems.Add(_vmTable[pp]);
            }
        }

        protected override void ExecuteDelete()
        {
            foreach (PatternPlacementViewModel pp in SelectedItems)
            {
                RemovePattern(pp.Source);
            }
        }

        protected override void ExecuteSelectAll()
        {
            ClearSelection();
            SelectItems(PlacedPatterns);
        }

        protected override void OnLastPressedItemChanged()
        {
            if (LastPressedItem is PatternPlacementViewModel ppvm)
            {
                Song.CurrentPattern = ppvm.Pattern;
            }

            base.OnLastPressedItemChanged();
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
