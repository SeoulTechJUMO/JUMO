using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace JUMO.UI
{
    public class PianoRollViewModel : MusicalCanvasWorkspaceViewModel
    {
        private Score _score;
        private readonly Dictionary<Note, NoteViewModel> _vmTable = new Dictionary<Note, NoteViewModel>();
        private RelayCommand _cutCommand;
        private RelayCommand _copyCommand;
        private RelayCommand _pasteCommand;

        #region Commands

        public override RelayCommand CutCommand => _cutCommand ?? (_cutCommand = new RelayCommand(_=> Cut(), _ => SelectedItems.Count > 0));
        public override RelayCommand CopyCommand => _copyCommand ?? (_copyCommand = new RelayCommand(_ => Copy(), _ => SelectedItems.Count > 0));
        public override RelayCommand PasteCommand => _pasteCommand ?? (_pasteCommand = new RelayCommand(_ => Paste(), _ => Storage.Instance.CurrentClip != null));

        public void Cut()
        {
            Storage.Instance.CurrentClip = new ObservableCollection<IMusicalItem>(SelectedItems);
            foreach (NoteViewModel note in SelectedItems)
            {
                RemoveNote(note.Source);
            }
        }

        public void Copy()
        {
            Storage.Instance.CurrentClip = new ObservableCollection<IMusicalItem>(SelectedItems);
        }

        public void Paste()
        {
            int start = Sequencer.Position;
            int firstStart = 0;

            SelectedItems.Clear();
            foreach(NoteViewModel note in Storage.Instance.CurrentClip)
            {
                if (note == Storage.Instance.CurrentClip.ElementAt(0)) { firstStart = note.Start; }
                Note Insert = new Note(note.Value, note.Velocity, note.Start - firstStart + start, note.Length);
                AddNote(Insert);
                SelectedItems.Add(Insert);
            }
        }

        #endregion

        public Score Score
        {
            get => _score;
            private set
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

                OnPropertyChanged(nameof(Score));
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
            Plugin.Disposed += OnPluginDisposed;

            Score = Song.Current.CurrentPattern[Plugin];
            Song.Current.PropertyChanged += CurrentSong_PropertyChanged;

            GridStep = Song.Denominator >= 4 ? 4 : 2;
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

        private void OnPluginDisposed(object sender, EventArgs e) => CloseCommand?.Execute(null);
    }
}
