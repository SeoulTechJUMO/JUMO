using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
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
        private readonly Song _song;
        private readonly MidiToolkit.MidiInternalClock _clock = new MidiToolkit.MidiInternalClock();
        private readonly List<IEnumerator<long>> _trackEnumerators = new List<IEnumerator<long>>();
        private readonly BlockingCollection<Pattern> _patternQueue = new BlockingCollection<Pattern>();
        private readonly List<PatternSequencer> _playingPatterns = new List<PatternSequencer>();

        private readonly object _lock = new object();

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

                lock (_lock)
                {
                    wasPlaying = IsPlaying;

                    Stop();
                    _clock.SetTicks((int)value);
                }

                lock (_lock)
                {
                    if (wasPlaying)
                    {
                        Continue();
                    }
                }

                OnPropertyChanged(nameof(Position));
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                lock (_lock)
                {
                    if (_isPlaying != value)
                    {
                        _isPlaying = value;
                        OnPropertyChanged(nameof(IsPlaying));
                    }
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

            new Thread(PatternSequencerConsumer) { Name = "PatternSequencer Consumer for MasterSequencer" }.Start();
        }

        public void Start()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            lock (_lock)
            {
                Stop();
                Position = 0;
                Continue();
            }
        }

        public void Continue()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            lock (_lock)
            {
                Stop();
                _trackEnumerators.Clear();

                foreach (Track track in _song.Tracks)
                {
                    _trackEnumerators.Add(track.GetTickIterator(this, Position).GetEnumerator());
                }

                UpdateTimingProperties();
                _clock.Start();

                IsPlaying = true;
            }
        }

        public void Stop()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            lock (_lock)
            {
                if (!IsPlaying)
                {
                    return;
                }

                _clock.Stop();
                _playingPatterns.Clear();
                // TODO: stop all sounds (NoteOff)

                IsPlaying = false;
            }
        }

        internal void EnqueuePattern(Pattern pattern)
        {
            _patternQueue.Add(pattern);
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

        private void PatternSequencerConsumer()
        {
            while (!_patternQueue.IsCompleted)
            {
                Pattern pattern = null;

                try
                {
                    pattern = _patternQueue.Take();
                }
                catch (InvalidOperationException) { }

                if (pattern != null)
                {
                    _playingPatterns.Add(new PatternSequencer(this, pattern));
                }
            }
        }

        private void OnSongPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateTimingProperties();
        }

        private void OnClockTick(object sender, EventArgs e)
        {
            lock (_lock)
            {
                if (!IsPlaying)
                {
                    return;
                }

                foreach (IEnumerator<long> enumerator in _trackEnumerators)
                {
                    enumerator.MoveNext();
                }
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
                lock (_lock)
                {
                    Stop();
                    _clock.Dispose();
                    _patternQueue.CompleteAdding();
                    _patternQueue.Dispose();
                }
            }

            _isDisposed = true;
        }

        public void Dispose() => Dispose(true);

        #endregion
    }
}
