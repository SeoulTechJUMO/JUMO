using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using MidiToolkit = Sanford.Multimedia.Midi;

namespace JUMO.Playback
{
    public enum PlaybackMode
    {
        Pattern,
        Song
    }

    public class MasterSequencer : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        private readonly Song _song;
        private readonly MidiToolkit.MidiInternalClock _clock = new MidiToolkit.MidiInternalClock();

        private readonly List<IEnumerator<long>> _trackEnumerators = new List<IEnumerator<long>>();
        private int _numOfPlayingTracks;

        private readonly BlockingCollection<Action> _workQueue = new BlockingCollection<Action>();

        private readonly Thread _workerThread;

        private bool _isDisposed = false;
        private bool _isPlaying = false;

        #endregion

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

                EnqueueWork(() =>
                {
                    bool wasPlaying;

                    wasPlaying = IsPlaying;

                    Stop();
                    _clock.SetTicks((int)value);
                    if (wasPlaying)
                    {
                        Continue();
                    }

                    OnPropertyChanged(nameof(Position));
                });
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                EnqueueWork(() =>
                {
                    if (_isPlaying != value)
                    {
                        _isPlaying = value;
                        OnPropertyChanged(nameof(IsPlaying));
                    }
                });
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

        internal MasterSequencer(Song song)
        {
            _song = song ?? throw new ArgumentNullException(nameof(song));

            _song.PropertyChanged += OnSongPropertyChanged;
            _clock.Tick += OnClockTick;

            UpdateTimingProperties();

            _workerThread = new Thread(Worker)
            {
                Name = "MasterSequencer Worker",
                IsBackground = true
            };
            _workerThread.Start();
        }

        public void Start()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            EnqueueWork(() =>
            {
                Stop();
                Position = 0;
                Continue();
            });
        }

        public void Continue()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            EnqueueWork(() =>
            {
                Stop();
                _trackEnumerators.Clear();

                foreach (Track track in _song.Tracks)
                {
                    _trackEnumerators.Add(track.GetTickIterator(this, Position).GetEnumerator());
                }

                _numOfPlayingTracks = Song.NumOfTracks;

                UpdateTimingProperties();
                _clock.Continue();

                IsPlaying = true;
            });
        }

        public void Stop()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            EnqueueWork(() =>
            {
                if (!IsPlaying)
                {
                    return;
                }

                _clock.Stop();

                // TODO: stop all sounds (NoteOff)

                IsPlaying = false;

                System.Diagnostics.Debug.WriteLine("MasterSequencer: Playback stopped");
            });
        }

        internal void EnqueuePattern(Pattern pattern) => new PatternSequencer(this, pattern);

        internal void HandleFinishedTrack()
        {
            EnqueueWork(() =>
            {
                _numOfPlayingTracks--;

                if (_numOfPlayingTracks == 0)
                {
                    Start();
                }
            });
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

        private void EnqueueWork(Action work)
        {
            if (Thread.CurrentThread.ManagedThreadId == _workerThread.ManagedThreadId)
            {
                work?.Invoke();
            }
            else
            {
                _workQueue.Add(work);
            }
        }

        private void Worker()
        {
            while (!_workQueue.IsCompleted)
            {
                Action action = null;

                try
                {
                    action = _workQueue.Take();
                }
                catch (InvalidOperationException) { }

                action?.Invoke();
            }
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
                Stop();
                _clock.Dispose();
                _workQueue.CompleteAdding();
            }

            _isDisposed = true;
        }

        public void Dispose() => Dispose(true);

        #endregion
    }
}
