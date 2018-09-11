using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using JUMO;

namespace ChordMagicianTest
{
    class Chord : INotifyPropertyChanged
    {
        private string _Name;
        private List<Note> _Notes;

        public string Name
        {
            get => _Name;
            set
            {
                _Name = Name;
                OnPropertyChanged(nameof(Name));
            }
        }

        public List<Note> Notes
        {
            get => _Notes;
            set
            {
                _Notes = Notes;
                OnPropertyChanged(nameof(Notes));
            }
        }

        public Chord(string name, List<Note> notes)
        {
            Name = name;
            Notes = notes;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
