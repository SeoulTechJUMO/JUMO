using System;
using System.Collections.Generic;
using System.ComponentModel;
using NAudio.Wave;

namespace JUMO.Audio
{
    public sealed class AudioManager : INotifyPropertyChanged
    {
        #region Singleton

        private static readonly Lazy<AudioManager> _instance = new Lazy<AudioManager>(() => new AudioManager());

        public static AudioManager Instance => _instance.Value;

        private AudioManager() => PopulateAudioOutputDevices();

        #endregion

        private readonly List<AudioOutputDevice> _outputDevices = new List<AudioOutputDevice>();
        private AudioOutputDevice _currentOutputDevice = null;
        private AudioOutputEngine _outputEngine = null;
        private bool _isInitialized = false;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OutputDeviceChanged;

        public IEnumerable<AudioOutputDevice> OutputDevices
        {
            get
            {
                foreach (AudioOutputDevice device in _outputDevices)
                {
                    yield return device;
                }
            }
        }

        public AudioOutputDevice CurrentOutputDevice
        {
            get => _currentOutputDevice;
            set
            {
                if (!Equals(_currentOutputDevice, value))
                {
                    _currentOutputDevice = value;
                    _outputEngine?.Dispose();
                    _outputEngine = value == null ? null : new AudioOutputEngine(value);

                    if (_isInitialized)
                    {
                        Properties.Settings.Default.AudioDeviceIndex = _outputDevices.IndexOf(value);
                        Properties.Settings.Default.Save();
                    }

                    OnPropertyChanged(nameof(CurrentOutputDevice));
                    OutputDeviceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool IsPlaying => _outputEngine?.IsPlaying ?? false;

        public void AddMixerInput(ISampleProvider input)
            => _outputEngine?.AddMixerInput(input);

        public void Play() => _outputEngine?.Play();

        public void Stop() => _outputEngine?.Stop();

        private void PopulateAudioOutputDevices()
        {
            int numOfWaveOutDevices = WaveOut.DeviceCount;

            for (int i = 0; i < numOfWaveOutDevices; i++)
            {
                _outputDevices.Add(new WaveOutDevice(i));
            }

            foreach (var dsDev in DirectSoundOut.Devices)
            {
                _outputDevices.Add(new DirectSoundOutputDevice(dsDev.Guid, dsDev.Description));
            }

            foreach (var asio in AsioOut.GetDriverNames())
            {
                _outputDevices.Add(new AsioOutputDevice(asio));
            }

            int defaultDeviceIndex = Math.Max(0, Math.Min(Properties.Settings.Default.AudioDeviceIndex, _outputDevices.Count - 1));

            CurrentOutputDevice = _outputDevices[defaultDeviceIndex];
            _isInitialized = true;
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
