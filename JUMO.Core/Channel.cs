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
        public MixerChannel(string name, bool IsMaster=false)
        {
            Name = name;
            Mixer.ReadFully = true;

            //마스터 채널일때 초기화
            if (IsMaster) { this.IsMaster = true; }

            //일반 채널에서 초기화
            _VolumeSample = new VolumeSampleProvider(Mixer);
            _VolumeMeterSample = new MeteringSampleProvider(VolumeSample,1000);
            Plugins = EffectManager.Plugins;
            Volume = 0.8f;
            _VolumeMeterSample.StreamVolume += OnPostVolumeMeter;
            inSamples = VolumeMeterSample;
        }

        void OnPostVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            LeftVolume = e.MaxSampleValues[0];
            RightVolume = e.MaxSampleValues[1];
        }

        private float _LeftVolume;
        public float LeftVolume
        {
            get => _LeftVolume;
            set
            {
                _LeftVolume = value;
                OnPropertyChanged(nameof(LeftVolume));
            }
        }
        private float _RightVolume;
        public float RightVolume
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
        /// 채널의 음소거 여부
        /// </summary>
        private bool _IsMuted = false;
        public bool IsMuted
        {
            get => _IsMuted;
            set
            {
                _IsMuted = value;

                if (_IsMuted)
                {
                    //뮤트 처리
                    
                }
                else
                {
                    //뮤트 해제

                }

                OnPropertyChanged(nameof(IsMuted));
            }
        }

        /// <summary>
        /// 채널의 솔로 여부
        /// </summary>
        public bool IsSolo = false;

        //마스터 여부
        public readonly bool IsMaster = false;

        //이팩트 플러그인 관리자
        public EffectPluginManager EffectManager = new EffectPluginManager();

        //내부 믹서 프로바이더, 채널 최종 출력
        private MixingSampleProvider Mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));

        //내부 이팩트간의 샘플프로바이더 
        private ISampleProvider inSamples;

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
        private MeteringSampleProvider _VolumeMeterSample;
        public MeteringSampleProvider VolumeMeterSample
        {
            get => _VolumeMeterSample;
            set
            {
                _VolumeMeterSample = value;
                OnPropertyChanged(nameof(VolumeMeterSample));
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

        public void AddEffect()
        {
            if (Plugins.Count != 0)
            {
                EffectManager.AddPlugin(this, Plugins[Plugins.Count - 1].SampleProvider, null);
            }
            else
            {
                EffectManager.AddPlugin(this, inSamples, null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
