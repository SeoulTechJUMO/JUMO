using System;
using System.ComponentModel;

namespace JUMO.UI
{
    public class PatternPlacementViewModel : IMusicalItem, INotifyPropertyChanged
    {
        private bool _updating = false;

        private long _start;
        private long _length;
        private int _trackIndex;

        public event PropertyChangedEventHandler PropertyChanged;

        public PatternPlacement Source { get; }

        public Pattern Pattern { get; }

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
                throw new NotSupportedException();
            }
        }

        public int TrackIndex
        {
            get => _trackIndex;
            set
            {
                if (_trackIndex != value)
                {
                    _trackIndex = value;
                    OnPropertyChanged(nameof(TrackIndex));
                }
            }
        }

        public PatternPlacementViewModel(PatternPlacement source)
        {
            Source = source;
            Start = Source.Start;
            // 플레이리스트에 배치된 패턴의 길이를 변경하는 것은 아직 구현되지 않음.
            // Length = Source.Length;
            _length = Source.Length;
            Pattern = Source.Pattern;
            TrackIndex = Source.TrackIndex;

            Source.PropertyChanged += OnSourcePropertyChanged;
        }

        public void UpdateSource()
        {
            _updating = true;

            Source.Start = Start;
            // 플레이리스트에 배치된 패턴의 길이를 변경하는 것은 아직 구현되지 않음.
            // Source.Length = Length;
            Source.TrackIndex = TrackIndex;

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
