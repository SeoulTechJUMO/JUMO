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

        private bool _isDisposed = false;
        private long _position = 0;

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
            get => _position;
            set
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(nameof(MasterSequencer));
                }

                throw new NotImplementedException();

                OnPropertyChanged(nameof(Position));
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
            throw new NotImplementedException();
        }

        public void Start()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            throw new NotImplementedException();
        }

        public void Continue()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            throw new NotImplementedException();
        }

        public void Stop()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MasterSequencer));
            }

            throw new NotImplementedException();
        }

        private void UpdateTimingProperties()
        {
            if (!_isDisposed)
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
