using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUMO.Vst;
using NAudio.Wave;
using NAudio.Gui;
using NAudio.Wave.SampleProviders;

namespace JUMO
{
    public class Channel : INotifyPropertyChanged
    {
        /// <summary>
        /// 채널의 이름
        /// </summary>
        private string _Name = "";
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// 채널의 번호
        /// </summary>
        private int _ChannelNumber;
        public int ChannelNumber
        {
            get => _ChannelNumber;
            set
            {
                _ChannelNumber = value;
                OnPropertyChanged(nameof(ChannelNumber));
            }
        }

        /// <summary>
        /// 채널의 음소거 여부
        /// </summary>
        private bool _IsMuted = false;
        public bool IsMuted
        {
            get => _IsMuted;
            set
            {
                _IsMuted = value;
                OnPropertyChanged(nameof(IsMuted));
            }
        }

        /// <summary>
        /// 채널의 솔로 여부
        /// </summary>

        private bool _IsSolo = false;
        public bool IsSolo
        {
            get => _IsSolo;
            set
            {
                _IsSolo = value;
                OnPropertyChanged(nameof(IsSolo));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class MixerChannel : Channel
    {
        public MixerChannel(string name, bool IsMaster=false)
        {
            Name = name;
            if(IsMaster)
            {
                //마스터 채널일때 초기화
                this.IsMaster = true;
                _VolumeSample = new VolumeSampleProvider(Mixer);
                _VolumeMeter = new MeteringSampleProvider(Mixer,10);
                Plugins = EffectManager.Plugins; 
            }
            else
            {
                //일반 채널에서 초기화
                _VolumeSample = new VolumeSampleProvider(Mixer);
                _VolumeMeter = new MeteringSampleProvider(Mixer, 10);
                Plugins = EffectManager.Plugins;
            }
        }

        private bool IsMaster = false;

        private EffectPluginManager EffectManager = new EffectPluginManager();

        private MixingSampleProvider Mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));

        private VolumeSampleProvider _VolumeSample;
        public VolumeSampleProvider VolumeSample
        {
            get => _VolumeSample;
        }

        private MeteringSampleProvider _VolumeMeter;
        public MeteringSampleProvider VolumeMeter
        {
            get => _VolumeMeter;
            set
            {
                _VolumeMeter = value;
                OnPropertyChanged(nameof(VolumeMeter));
            }
        }

        //채널 볼륨
        public double Volume
        {
            get => _VolumeSample.Volume;
            set
            {
                _VolumeSample.Volume = (float)value;
                System.Diagnostics.Debug.WriteLine(_VolumeSample.Volume);
                OnPropertyChanged(nameof(Volume));
            }
        }

        /// <summary>
        /// 채널에 로드된 vst
        /// </summary>
        private ObservableCollection<Plugin> _Plugins = new ObservableCollection<Plugin>();
        public ObservableCollection<Plugin> Plugins
        {
            get => _Plugins;
            set
            {
                _Plugins = value;
                OnPropertyChanged(nameof(Plugins));
            }
        }


        //채널 솔로 활성화 & 비활성화
        public void ToggleSolo(MixerChannel CurrentChannel, List<MixerChannel> ChannelList)
        {
            if (!IsSolo)
            {
                //다른 채널 isMuted 활성화
                foreach (MixerChannel c in ChannelList)
                {
                    if(c != CurrentChannel && !c.IsMaster)
                    {
                        c.IsMuted = true;
                    }
                }
                IsSolo = true;
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
                IsSolo = false;
            }
        }

        public void ToggleMute()
        {
            //음소거 처리
        }

        public void MixerSendInput(ISampleProvider input)
        {
            Mixer.AddMixerInput(input);
        }

        public void MixerInputDisable(ISampleProvider input)
        {
            Mixer.RemoveMixerInput(input);
        }
    }
}
