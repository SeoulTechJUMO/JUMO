using System.Collections.Generic;
using JUMO.Audio;

namespace JUMO.UI.ViewModels
{
    public class AudioSettingsViewModel : SettingsGroupViewModel
    {
        public override string DisplayName => "오디오 출력";

        public IEnumerable<AudioOutputDevice> OutputDevices => AudioManager.Instance.OutputDevices;

        public AudioOutputDevice CurrentOutputDevice
        {
            get => AudioManager.Instance.CurrentOutputDevice;
            set
            {
                AudioManager.Instance.CurrentOutputDevice = value;
                OnPropertyChanged(nameof(CurrentOutputDevice));
            }
        }

        public AudioOutputDevice SelectedOutputDevice { get; set; }

        public AudioSettingsViewModel()
        {
            SelectedOutputDevice = CurrentOutputDevice;
        }

        public override void SaveSettings()
        {
            CurrentOutputDevice = SelectedOutputDevice;
        }
    }
}
