using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace JUMO
{
    public class Score : ObservableCollection<Note>
    {
        private long _length = 0;

        public Pattern Pattern { get; }

        public long Length
        {
            get => _length;
            private set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Length)));
                }
            }
        }

        public event EventHandler NotePropertyChanged;

        public Score(Pattern pattern)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Note note in e.OldItems)
                {
                    note.PropertyChanged -= OnNotePropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (Note note in e.NewItems)
                {
                    note.PropertyChanged += OnNotePropertyChanged;
                }
            }

            UpdateLength();
            base.OnCollectionChanged(e);
        }

        private void UpdateLength() => Length = this.Select(note => note.Start + note.Length).DefaultIfEmpty(0L).Max();

        private void OnNotePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateLength();
            NotePropertyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
