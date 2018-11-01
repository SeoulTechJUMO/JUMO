using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using JUMO.Vst;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using JUMO.Mixer;

namespace JUMO
{
    public class MixerChannel : INotifyPropertyChanged
    {
        private static MixerChannel _masterChannel;

        //내부 믹서 프로바이더
        private readonly MixingSampleProvider _mixingSampleProvider = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
        private readonly EffectChainSampleProvider _effectChainSampleProvider;
        private readonly VolumePanningSampleProvider _volumePanningSampleProvider;

        //이팩트 플러그인 관리자
        private EffectPluginManager _effectManager = new EffectPluginManager();

        private float _leftVolume;
        private float _rightVolume;
        private string _name;

        #region Properties

        /// <summary>
        /// 마스터 채널 여부
        /// </summary>
        public bool IsMaster { get; }

        /// <summary>
        /// 좌 볼륨 미터 값
        /// </summary>
        public float LeftVolume
        {
            get => _leftVolume;
            set
            {
                _leftVolume = value;
                OnPropertyChanged(nameof(LeftVolume));
            }
        }

        /// <summary>
        /// 우 볼륨 미터 값
        /// </summary>
        public float RightVolume
        {
            get => _rightVolume;
            set
            {
                _rightVolume = value;
                OnPropertyChanged(nameof(RightVolume));
            }
        }

        /// <summary>
        /// 채널의 패닝 값 (밸런싱)
        /// </summary>
        public float Panning
        {
            get => _volumePanningSampleProvider.Panning;
            set
            {
                _volumePanningSampleProvider.Panning = value;
                OnPropertyChanged(nameof(Panning));
            }
        }

        /// <summary>
        /// 채널의 볼륨 값
        /// </summary>
        public double Volume
        {
            get => _volumePanningSampleProvider.Volume;
            set
            {
                _volumePanningSampleProvider.Volume = (float)value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        /// <summary>
        /// 채널의 이름
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// 채널의 음소거 여부
        /// </summary>
        public bool IsMuted
        {
            get => _volumePanningSampleProvider.Mute;
            set
            {
                _volumePanningSampleProvider.Mute = value;
                OnPropertyChanged(nameof(IsMuted));
            }
        }

        /// <summary>
        /// 채널의 솔로 여부
        /// </summary>
        public bool IsSolo { get; internal set; } = false;

        /// <summary>
        /// 채널의 현재 최종 출력
        /// </summary>
        public ISampleProvider ChannelOut { get; }

        /// <summary>
        /// 채널에 로드된 VST 리스트
        /// </summary>
        public ObservableCollection<EffectPlugin> Plugins => _effectManager.Plugins;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public MixerChannel(string name, bool isMaster=false)
        {
            Name = name;
            _mixingSampleProvider.ReadFully = true;

            //마스터 채널일때 초기화
            if (isMaster)
            {
                IsMaster = true;
                _masterChannel = this;
            }

            //일반 채널에서 초기화
            _effectChainSampleProvider = new EffectChainSampleProvider(_mixingSampleProvider, _effectManager.Plugins);
            _volumePanningSampleProvider = new VolumePanningSampleProvider(_effectChainSampleProvider, 1000);
            ChannelOut = _volumePanningSampleProvider;
            Volume = 0.8f;
            IsMuted = false;

            //샘플 변경 확인시 호출 메소드 등록
            _volumePanningSampleProvider.StreamVolume += OnPostVolumeMeter;
        }

        public void MixerAddInput(ISampleProvider input)
        {
            if (!IsMaster && _mixingSampleProvider.MixerInputs.Count() == 0)
            {
                _masterChannel.MixerAddInput(ChannelOut);
            }

            _mixingSampleProvider.AddMixerInput(input);
        }

        public void MixerRemoveInput(ISampleProvider input)
        {
            _mixingSampleProvider.RemoveMixerInput(input);

            if (!IsMaster && _mixingSampleProvider.MixerInputs.Count() == 0)
            {
                _masterChannel.MixerRemoveInput(ChannelOut);
                LeftVolume = 0;
                RightVolume = 0;
            }
        }

        public EffectPlugin AddEffect(string pluginPath) => _effectManager.AddPlugin(pluginPath, this, null);

        public EffectPlugin ReplaceEffect(string pluginPath, EffectPlugin oldPlugin) => _effectManager.AddPlugin(pluginPath, this, null ,true, oldPlugin);

        public void MoveUp(int idx) => _effectManager.MoveUp(idx);
        public void MoveDown(int idx) => _effectManager.MoveDown(idx);

        public void RemoveEffect(EffectPlugin plugin) => _effectManager.RemovePlugin(plugin);

        public void UnloadAllEffects() => _effectManager.UnloadAll();

        //볼륨 이벤트 발생시 실행 메소드
        private void OnPostVolumeMeter(object sender, VolumePanningSampleProvider.StreamVolumeEventArgs e)
        {
            LeftVolume = e.MaxSampleValues[0];
            RightVolume = e.MaxSampleValues[1];
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
