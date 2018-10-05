using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiToolkit = Sanford.Multimedia.Midi;

namespace JUMO.Playback
{
    enum PlaybackMode
    {
        Pattern,
        Song
    }

    class MasterSequencer : INotifyPropertyChanged, IDisposable
    {
        private readonly MidiToolkit.MidiInternalClock _clock = new MidiToolkit.MidiInternalClock();
        private readonly Song _song;
        private readonly List<IEnumerator<long>> _trackEnumerators = new List<IEnumerator<long>>();

        private bool _isDisposed = false;
        private bool _isPlaying = false;

        #region Properties

        public int TimeResolution
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(nameof(MasterSequencer));
                }

                return _clock.Ppqn;
            }
            private set
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(nameof(MasterSequencer));
                }

                _clock.Ppqn = value;
            }
        }

        public int MidiTempo
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(nameof(MasterSequencer));
                }

                return _clock.Tempo;
            }
            private set
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(nameof(MasterSequencer));
                }

                _clock.Tempo = value;
            }
        }

        public long Position
        {
            get => _clock.Ticks;
            set
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(nameof(MasterSequencer));
                }

                bool wasPlaying;
                
                wasPlaying = IsPlaying;
                Stop();
                _clock.SetTicks((int)value);

                if (wasPlaying)
                {
                    Continue();
                }

                OnPropertyChanged(nameof(Position));
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged(nameof(IsPlaying));
                }
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Tick
        {
            add => _clock.Tick += value;
            remove => _clock.Tick -= value;
        }

        #endregion

        public MasterSequencer(Song song)
        {
            _song = song ?? throw new ArgumentNullException(nameof(song));

            _song.PropertyChanged += OnSongPropertyChanged;
            _clock.Tick += OnClockTick;

            UpdateTimingProperties();
        }

        public void Start()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            Stop();
            Position = 0;
            Continue();
        }

        public void Continue()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            Stop();
            _trackEnumerators.Clear();

            foreach (Track track in _song.Tracks)
            {
                _trackEnumerators.Add(track.GetTickIterator(Position).GetEnumerator());
            }

            UpdateTimingProperties();
            _clock.Start();

            IsPlaying = true;
        }

        public void Stop()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            if (!IsPlaying)
            {
                return;
            }

            _clock.Stop();

            IsPlaying = false;
        }

        private void UpdateTimingProperties()
        {
            if (_isDisposed)
            {
                return;
            }

            TimeResolution = _song.TimeResolution;
            MidiTempo = _song.MidiTempo;
        }

        private void OnSongPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateTimingProperties();
        }

        private void OnClockTick(object sender, EventArgs e)
        {
            if (!IsPlaying)
            {
                return;
            }

            foreach (IEnumerator<long> enumerator in _trackEnumerators)
            {
                enumerator.MoveNext();
            }

            OnPropertyChanged(nameof(Position));
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _clock.Dispose();
            }

            _isDisposed = true;
        }

        public void Dispose() => Dispose(true);

        #endregion
    }
}
