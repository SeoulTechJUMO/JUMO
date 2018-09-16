using System.ComponentModel;

namespace JUMO.UI
{
    public class NoteViewModel : IMusicalItem, INotifyPropertyChanged
    {
        private bool _updating = false;

        private byte _value;
        private long _start;
        private long _length;
        private byte _velocity;

        public event PropertyChangedEventHandler PropertyChanged;

        public Note Source { get; }

        public byte Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public long Start
        {
            get => _start;
            set
            {
                if (_start != value)
                {
                    _start = value;
                    OnPropertyChanged(nameof(Start));
                }
            }
        }

        public long Length
        {
            get => _length;
            set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        public byte Velocity
        {
            get => _velocity;
            set
            {
                if (_velocity != value)
                {
                    _velocity = value;
                    OnPropertyChanged(nameof(Velocity));
                }
            }
        }

        public NoteViewModel(Note source)
        {
            Source = source;
            Value = Source.Value;
            Start = Source.Start;
            Length = Source.Length;
            Velocity = Source.Velocity;

            Source.PropertyChanged += OnSourcePropertyChanged;
        }

        public void UpdateSource()
        {
            _updating = true;

            Source.Value = Value;
            Source.Start = Start;
            Source.Length = Length;
            Source.Velocity = Velocity;

            _updating = false;
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_updating)
            {
                return;
            }

            var srcValue = sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
            GetType().GetProperty(e.PropertyName)?.SetValue(this, srcValue);
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
