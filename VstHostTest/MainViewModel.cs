using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Jacobi.Vst.Interop.Host;
using JUMO.Audio;
using JUMO.Vst;

namespace VstHostTest
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<AudioOutputDevice> AudioOutputDevices => AudioManager.Instance.OutputDevices;

        public AudioOutputDevice CurrentAudioOutputDevice
        {
            get => AudioManager.Instance.CurrentOutputDevice;
            private set
            {
                AudioManager.Instance.CurrentOutputDevice = value;
                OnPropertyChanged(nameof(CurrentAudioOutputDevice));
            }
        }

        public AudioOutputDevice SelectedAudioOutputDevice { get; set; }

        public void ChangeCurrentAudioOutputDevice()
        {
            CurrentAudioOutputDevice = SelectedAudioOutputDevice;
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainViewModel()
        {
        }
    }
}
