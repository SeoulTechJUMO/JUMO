using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUMO.Audio;

namespace JUMO
{
    public sealed class MixerManager : INotifyPropertyChanged
    {
        #region Singleton

        private static readonly Lazy<MixerManager> _instance = new Lazy<MixerManager>(() => new MixerManager());

        public static MixerManager Instance => _instance.Value;

        #endregion

        public MixerManager()
        {
            for (int i = 0; i < 100; i++)
            {
                if (i == 0)
                {
                    MixerChannels.Add(new MixerChannel("마스터", true));
                    AudioManager.Instance.AddMixerInput(MixerChannels[i].VolumeSample);
                }
                else
                {
                    MixerChannels.Add(new MixerChannel($"채널 {i}"));
                    MixerChannels[0].MixerSendInput(MixerChannels[i].VolumeSample);
                }
            }
            AudioManager.Instance.OutputDeviceChanged += AudioOutputDeviceChanged;
        }

        //믹서 채널
        private ObservableCollection<MixerChannel> _MixerChannels = new ObservableCollection<MixerChannel>();
        public ObservableCollection<MixerChannel> MixerChannels
        {
            get => _MixerChannels;
            set
            {
                _MixerChannels = value;
                OnPropertyChanged(nameof(MixerChannels));
            }
        }

        private void AudioOutputDeviceChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PluginManager: Audio output device has changed.");

            if (AudioManager.Instance.CurrentOutputDevice == null)
            {
                return;
            }

            //현재 마스터 믹스를 해제하고 새로운 디바이스 믹서에 임포트
            AudioManager.Instance.DisposeMixerInput(MixerChannels[0].VolumeSample);
            AudioManager.Instance.AddMixerInput(MixerChannels[0].VolumeSample);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
