using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    class PlaybackTimeViewModel : INotifyPropertyChanged
    {
        private readonly Song _song;
        private readonly Playback.MasterSequencer _sequencer;

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
            _song = Song.Current;
            _sequencer = _song.Sequencer;

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
                long totalMilliseconds = (_song.MidiTempo / _song.TimeResolution) * _sequencer.Position / 1000;
                int totalSeconds = (int)(totalMilliseconds / 1000);

                Milliseconds = (int)(totalMilliseconds - totalSeconds * 1000);
                Minutes = totalSeconds / 60;
                Seconds = totalSeconds - Minutes * 60;

                int bars = (int)(_sequencer.Position / _ticksPerBar);
                Bars = bars + 1;
                Beats = (int)(_sequencer.Position - bars * _ticksPerBar) / _ticksPerBeat + 1;

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
