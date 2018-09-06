using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace JUMO
{
    public class Score : ObservableCollection<Note>
    {
        public Pattern Pattern { get; }

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

            base.OnCollectionChanged(e);
        }

        private void OnNotePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotePropertyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
