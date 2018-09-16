using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace JUMO.UI
{
    public class PianoRollViewModel : MusicalCanvasWorkspaceViewModel
    {
        private Score _score;
        private readonly Dictionary<Note, NoteViewModel> _vmTable = new Dictionary<Note, NoteViewModel>();

        private Score Score
        {
            get => _score;
            set
            {
                if (_score != null)
                {
                    _score.CollectionChanged -= OnScoreCollectionChanged;
                }

                _score = value;

                if (_score != null)
                {
                    _vmTable.Clear();
                    SelectedItems.Clear();

                    Notes = new ObservableCollection<NoteViewModel>();

                    foreach (Note note in _score)
                    {
                        AddNoteInternal(note);
                    }

                    _score.CollectionChanged += OnScoreCollectionChanged;
                }

                OnPropertyChanged(nameof(Notes));
            }
        }

        protected override double ZoomBase => 24.0;

        public override WorkspaceKey Key { get; }
        public override string DisplayName => $"피아노 롤: {Plugin.Name}";

        public Vst.Plugin Plugin { get; }

        public override IEnumerable<int> GridStepOptions { get; } = new[] { 1, 2, 3, 4, 6, 8, 12, 16 };

        public ObservableCollection<NoteViewModel> Notes { get; private set; }

        public PianoRollViewModel(Vst.Plugin plugin) : base()
        {
            Plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            Key = new PianoRollWorkspaceKey(plugin);

            Score = Song.Current.CurrentPattern[Plugin];
            Song.Current.PropertyChanged += CurrentSong_PropertyChanged;

            GridStep = Denominator >= 4 ? 4 : 2;
        }

        public void AddNote(Note note) => _score.Add(note);
        public void RemoveNote(Note note) => _score.Remove(note);

        private void AddNoteInternal(Note note)
        {
            NoteViewModel vm = new NoteViewModel(note);

            _vmTable.Add(note, vm);
            Notes?.Add(vm);
        }

        private void RemoveNoteInternal(Note note)
        {
            if (_vmTable.TryGetValue(note, out NoteViewModel vm))
            {
                Notes?.Remove(vm);
                _vmTable.Remove(note);
            }
        }

        private void CurrentSong_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);

            if (e.PropertyName.Equals(nameof(Song.CurrentPattern)))
            {
                Score = Song.Current.CurrentPattern[Plugin];
            }
        }

        private void OnScoreCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Note note in e.OldItems)
                {
                    RemoveNoteInternal(note);
                }
            }

            if (e.NewItems != null)
            {
                foreach (Note note in e.NewItems)
                {
                    AddNoteInternal(note);
                }
            }
        }
    }
}
