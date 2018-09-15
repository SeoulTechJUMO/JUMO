using System.ComponentModel;

namespace JUMO.UI
{
    class NoteViewModel : IMusicalItem, INotifyPropertyChanged
    {
        private readonly Note _source;
        private byte _value;
        private long _start;
        private long _length;
        private byte _velocity;

        public event PropertyChangedEventHandler PropertyChanged;

        public byte Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public long Start
        {
            get => _start;
            set
            {
                _start = value;
                OnPropertyChanged(nameof(Start));
            }
        }

        public long Length
        {
            get => _length;
            set
            {
                _length = value;
                OnPropertyChanged(nameof(Length));
            }
        }

        public byte Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                OnPropertyChanged(nameof(Velocity));
            }
        }

        public NoteViewModel(Note source)
        {
            _source = source;
            Value = _source.Value;
            Start = _source.Start;
            Length = _source.Length;
            Velocity = _source.Velocity;

            _source.PropertyChanged += OnSourcePropertyChanged;
        }

        public void UpdateSource()
        {
            _source.Value = Value;
            _source.Start = Start;
            _source.Length = Length;
            _source.Velocity = Velocity;
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var srcValue = sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
            GetType().GetProperty(e.PropertyName)?.SetValue(this, srcValue);
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
