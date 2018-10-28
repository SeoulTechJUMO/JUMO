using Jacobi.Vst.Core;
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

        public void ProcessEffect(VstAudioBuffer[] inBuffer, VstAudioBuffer[] outBuffer, int samplesPerBuffer)
        {
            if (FetchEvents(out VstEvent[] events))
            {
                PluginCommandStub.ProcessEvents(events);
            }

            unsafe
            {
                for (int bufIndex = 0; bufIndex < outBuffer.Length; bufIndex++)
                {
                    float* pOutBuf = ((IDirectBufferAccess32)outBuffer[bufIndex]).Buffer;

                    for (int i = 0; i < samplesPerBuffer; i++)
                    {
                        pOutBuf[i] = 0.0f;
                    }
                }

                PluginCommandStub.ProcessReplacing(inBuffer, outBuffer);

                for (int bufIndex = 0; bufIndex < inBuffer.Length; bufIndex++)
                {
                    float* pInBuf = ((IDirectBufferAccess32)inBuffer[bufIndex]).Buffer;
                    float* pOutBuf = ((IDirectBufferAccess32)outBuffer[bufIndex]).Buffer;

                    for (int i = 0; i < samplesPerBuffer; i++)
                    {
                        pOutBuf[i] = pOutBuf[i] * EffectMix + pInBuf[i] * (1 - EffectMix);
                    }
                }
            }
        }
    }
}
