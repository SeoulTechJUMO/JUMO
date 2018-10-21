using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUMO.Audio;
using JUMO.Vst;

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
                    MixerChannels.Add(new MixerChannel("마스터", i, true));
                    AudioManager.Instance.AddMixerInput(MixerChannels[i].VolumeSample);
                }
                else
                {
                    MixerChannels.Add(new MixerChannel($"채널 {i}", i));
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

        //채널 솔로 활성화 & 비활성화
        public void ToggleSolo(MixerChannel CurrentChannel, List<MixerChannel> ChannelList)
        {
            if (!CurrentChannel.IsSolo)
            {
                //다른 채널 isMuted 활성화
                foreach (MixerChannel c in ChannelList)
                {
                    if (c != CurrentChannel && !c.IsMaster)
                    {
                        c.IsMuted = true;
                    }
                }
                CurrentChannel.IsSolo = true;
            }
            else
            {
                //다른 채널 isMuted 활성화 해제
                foreach (MixerChannel c in ChannelList)
                {
                    if (c != CurrentChannel && !c.IsMaster)
                    {
                        c.IsMuted = false;
                    }
                }
                CurrentChannel.IsSolo = false;
            }
        }

        //플러그인 채널 채인지
        public void ChangeChannel(Plugin plugin, int TargetChannelNum)
        {
            MixerChannels[plugin.ChannelNum].MixerInputDispose(plugin.SampleProvider);
            MixerChannels[TargetChannelNum].MixerSendInput(plugin.SampleProvider);
        }

        //오디오 디바이스가 바뀔경우
        private void AudioOutputDeviceChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MixerManager: Audio output device has changed.");

            if (AudioManager.Instance.CurrentOutputDevice == null)
            {
                return;
            }

            //새로운 디바이스 믹서에 임포트
            AudioManager.Instance.AddMixerInput(MixerChannels[0].VolumeSample);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
