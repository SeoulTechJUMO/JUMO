using Jacobi.Vst.Core.Host;
using NAudio.Wave;

namespace JUMO.Vst
{
    public class EffectPlugin : PluginBase
    {
        private readonly VstSampleProvider _vstSampleProvider;

        #region Properties

        public ISampleProvider SampleProvider => _vstSampleProvider;

        public float EffectMix
        {
            get => _vstSampleProvider.EffectMix;
            set
            {
                _vstSampleProvider.EffectMix = value;
                OnPropertyChanged(nameof(EffectMix));
            }
        }

        #endregion

        public EffectPlugin(string pluginPath, IVstHostCommandStub hostCmdStub, ISampleProvider source) : base(pluginPath, hostCmdStub)
        {
            _vstSampleProvider = new VstSampleProvider(this, source);
        }
    }
}
