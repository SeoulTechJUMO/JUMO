using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;

namespace JUMO.Vst
{
    public class EffectPlugin : PluginBase
    {
        private float _effectMix = 1.0f;

        #region Properties

        public float EffectMix
        {
            get => _effectMix;
            set
            {
                _effectMix = value;
                OnPropertyChanged(nameof(EffectMix));
            }
        }

        #endregion

        public EffectPlugin(string pluginPath) : base(pluginPath) { }

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
