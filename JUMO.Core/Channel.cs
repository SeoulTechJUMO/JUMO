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
using JUMO.Mixer;

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
            _VolumePanningSample = new VolumePanningProvider(Mixer,1000);
            Plugins = EffectManager.Plugins;
            Volume = 0.8f;
            IsMuted = false;
            //샘플 변경 확인시 호출 메소드 등록
            _VolumePanningSample.StreamVolume += OnPostVolumeMeter;
        }

        //볼륨 이벤트 발생시 실행 메소드
        void OnPostVolumeMeter(object sender, VolumePanningProvider.StreamVolumeEventArgs e)
        {
            LeftVolume = e.MaxSampleValues[0];
            RightVolume = e.MaxSampleValues[1];
        }


        /// <summary>
        /// 좌 볼륨 미터 값
        /// </summary>
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

        /// <summary>
        /// 우 볼륨 미터 값
        /// </summary>
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
        /// 채널의 패닝 값 (밸런싱)
        /// </summary>
        public float Panning
        {
            get => _VolumePanningSample.Panning;
            set
            {
                _VolumePanningSample.Panning = value;
                OnPropertyChanged(nameof(Panning));
            }
        }

        /// <summary>
        /// 채널의 볼륨 값
        /// </summary>
        public double Volume
        {
            get => _VolumePanningSample.Volume;
            set
            {
                _VolumePanningSample.Volume = (float)value;
                OnPropertyChanged(nameof(Volume));
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
        public bool IsMuted
        {
            get => _VolumePanningSample.Mute;
            set
            {
                _VolumePanningSample.Mute = value;
                OnPropertyChanged(nameof(IsMuted));
            }
        }

        /// <summary>
        /// 채널의 솔로 여부
        /// </summary>
        public bool IsSolo = false;

        /// <summary>
        /// 마스터 채널 여부
        /// </summary>
        public readonly bool IsMaster = false;

        //이팩트 플러그인 관리자
        public EffectPluginManager EffectManager = new EffectPluginManager();

        //내부 믹서 프로바이더
        private MixingSampleProvider Mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));

        //채널의 최종 출력
        public ISampleProvider ChannelOut { get; private set; }

        //통합 샘플 프로바이더
        private VolumePanningProvider _VolumePanningSample;
        public VolumePanningProvider VolumePanningSample
        {
            get => _VolumePanningSample;
            set
            {
                _VolumePanningSample = value;
            }
        }

        /// <summary>
        /// 채널에 로드된 vst 리스트
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
                EffectManager.AddPlugin(this, Mixer, null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
