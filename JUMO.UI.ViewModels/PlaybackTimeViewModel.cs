using System.ComponentModel;

namespace JUMO.UI.ViewModels
{
    public class PlaybackTimeViewModel : INotifyPropertyChanged
    {
        private readonly Song _song = Song.Current;
        private readonly Playback.MasterSequencer _sequencer = Playback.MasterSequencer.Instance;

        private int _ticksPerBeat;
        private int _ticksPerBar;

        public int Milliseconds { get; private set; } = 0;
        public int Seconds { get; private set; } = 0;
        public int Minutes { get; private set; } = 0;

        public int Beats { get; private set; } = 1;
        public int Bars { get; private set; } = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public PlaybackTimeViewModel()
        {
            UpdateTickUnits();

            _song.PropertyChanged += OnSongPropertyChanged;
            _sequencer.PropertyChanged += OnSequencerPropertyChanged;
        }

        private void UpdateTickUnits()
        {
            _ticksPerBeat = _song.TimeResolution * 4 / _song.Denominator;
            _ticksPerBar = _ticksPerBeat * _song.Numerator;
        }

        private void OnSongPropertyChanged(object sender, PropertyChangedEventArgs e) => UpdateTickUnits();

        private void OnSequencerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Playback.MasterSequencer.Position)) {
                int totalMilliseconds = (_song.MidiTempo / _song.TimeResolution) * _sequencer.Position / 1000;
                int totalSeconds = totalMilliseconds / 1000;

                Milliseconds = totalMilliseconds - totalSeconds * 1000;
                Minutes = totalSeconds / 60;
                Seconds = totalSeconds - Minutes * 60;

                int bars = _sequencer.Position / _ticksPerBar;
                Bars = bars + 1;
                Beats = (_sequencer.Position - bars * _ticksPerBar) / _ticksPerBeat + 1;

                OnPropertyChanged(nameof(Milliseconds));
                OnPropertyChanged(nameof(Seconds));
                OnPropertyChanged(nameof(Minutes));
                OnPropertyChanged(nameof(Beats));
                OnPropertyChanged(nameof(Bars));
            }
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
