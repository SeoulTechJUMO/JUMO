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
    public class MixerChannel : INotifyPropertyChanged
    {
        public MixerChannel(string name, int Number, bool IsMaster=false)
        {
            Name = name;
            ChannelNumber = Number;
            Mixer.ReadFully = true;

            if(IsMaster)
            {
                //마스터 채널일때 초기화
                this.IsMaster = true;
                _VolumeSample = new VolumeSampleProvider(Mixer);
                _VolumeMeter = new MeteringSampleProvider(Mixer,1000);
                Plugins = EffectManager.Plugins; 
            }
            else
            {
                //일반 채널에서 초기화
                _VolumeSample = new VolumeSampleProvider(Mixer);
                _VolumeMeter = new MeteringSampleProvider(Mixer, 1000);
                Plugins = EffectManager.Plugins;
            }

            Volume = 0.8f;
            _VolumeMeter.StreamVolume += OnPostVolumeMeter;
        }

        void OnPostVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            // we know it is stereo
            LeftVolume = e.MaxSampleValues[0];
            RightVolume = e.MaxSampleValues[1];
        }

        private double _LeftVolume;
        public double LeftVolume
        {
            get => _LeftVolume;
            set
            {
                _LeftVolume = value;
                OnPropertyChanged(nameof(LeftVolume));
            }
        }

        private double _RightVolume;
        public double RightVolume
        {
            get => _RightVolume;
            set
            {
                _RightVolume = value;
                OnPropertyChanged(nameof(RightVolume));
            }
        }


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

                //if (_IsMuted)
                //{
                //    //뮤트 처리
                //    tempVol = Volume;
                //    VolumeSample.Volume = 0f;
                //}
                //else
                //{
                //    //뮤트 해제
                //    VolumeSample.Volume = (float)tempVol;
                //}
                    
                OnPropertyChanged(nameof(IsMuted));
            }
        }

        //뮤트용 임시 볼륨
        public double tempVol = 0.8f;

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

        //마스터 여부
        public readonly bool IsMaster = false;

        //이팩트 플러그인 관리자
        private EffectPluginManager EffectManager = new EffectPluginManager();

        //내부 믹서 프로바이더
        private MixingSampleProvider Mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));

        //볼륨 샘플
        private VolumeSampleProvider _VolumeSample;
        public VolumeSampleProvider VolumeSample
        {
            get => _VolumeSample;
            set
            {
                _VolumeSample = value;
            }
        }

        //볼륨 Meter
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

        public void MixerSendInput(ISampleProvider input)
        {
            Mixer.AddMixerInput(input);
        }

        public void MixerInputDispose(ISampleProvider input)
        {
            Mixer.RemoveMixerInput(input);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
