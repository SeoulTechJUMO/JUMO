using System;
using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;
using System;

namespace JUMO.Vst
{
    class VstSampleProvider : ISampleProvider
    {
        private readonly Plugin _plugin;

        public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        public ISampleProvider Source { get; set; }
        public float EffectMix { get; set; }

        public VstSampleProvider(Plugin plugin)
        {
            _plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
        }

        public VstSampleProvider(Plugin plugin, ISampleProvider source)
        {
            Source = source;
            EffectMix = 1.0f;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int halfCount = count >> 1;

            VstAudioBufferManager inBufMgr = new VstAudioBufferManager(2, halfCount);
            VstAudioBufferManager outBufMgr = new VstAudioBufferManager(2, halfCount);

#pragma warning disable CS0618 // Type or member is obsolete
            VstAudioBuffer[] inBuf = inBufMgr.ToArray();
            VstAudioBuffer[] outBuf = outBufMgr.ToArray();
#pragma warning restore CS0618 // Type or member is obsolete

            if (_plugin.FetchEvents(out VstEvent[] events))
            {
                _plugin.PluginCommandStub.ProcessEvents(events);
            }

            float[] tempBuf = new float[count];

            if (Source != null)
            {
                Source.Read(tempBuf, offset, count);

                unsafe
                {
                    float* inLBuf = ((IDirectBufferAccess32)inBuf[0]).Buffer;
                    float* inRBuf = ((IDirectBufferAccess32)inBuf[1]).Buffer;

                    fixed (float* audioBuf = &tempBuf[0])
                    {
                        for (int i = 0, j = 0; i < halfCount; i++)
                        {
                            *(inLBuf + j++) = *(audioBuf + i);
                            *(inRBuf + j++) = *(audioBuf + i);
                        }
                    }
                }
            }

            _plugin.PluginCommandStub.ProcessReplacing(inBuf, outBuf);

            unsafe
            {
                float* vstLBuf = ((IDirectBufferAccess32)outBuf[0]).Buffer;
                float* vstRBuf = ((IDirectBufferAccess32)outBuf[1]).Buffer;

                fixed (float* audioBuf = &buffer[0])
                {
                    for (int i = 0, j = 0; i < halfCount; i++)
                    {
                        if (Source != null)
                        {
                            *(audioBuf + j) = (*(vstLBuf + i)) * EffectMix + tempBuf[j++] * (EffectMix - 1);
                            *(audioBuf + j) = (*(vstRBuf + i)) * EffectMix + tempBuf[j++] * (EffectMix - 1);
                        }
                        else
                        {
                            *(audioBuf + j++) = *(vstLBuf + i);
                            *(audioBuf + j++) = *(vstRBuf + i);
                        }
                    }
                }
            }

            inBufMgr.Dispose();
            outBufMgr.Dispose();

            return count;
        }
    }
}
