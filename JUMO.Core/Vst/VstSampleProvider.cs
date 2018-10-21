using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;

namespace JUMO.Vst
{
    class VstSampleProvider : ISampleProvider
    {
        private readonly IVstPluginCommandStub _cmdstub;

        public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        public ISampleProvider Source { get; set; }

        public VstSampleProvider(Plugin plugin)
        {
            _cmdstub = plugin.PluginCommandStub;
        }

        public VstSampleProvider(Plugin plugin, ISampleProvider source)
        {
            Source = source;
            _cmdstub = plugin.PluginCommandStub;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int halfCount = count >> 1;
            VstAudioBufferManager inBufMgr = new VstAudioBufferManager(2, halfCount);
            VstAudioBufferManager outBufMgr = new VstAudioBufferManager(2, halfCount);
            VstAudioBuffer[] inBuf = inBufMgr.ToArray();
            VstAudioBuffer[] outBuf = outBufMgr.ToArray();

            if (Source != null)
            {
                float[] tempBuf = new float[count];
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

            _cmdstub.ProcessReplacing(inBuf, outBuf);

            unsafe
            {
                float* vstLBuf = ((IDirectBufferAccess32)outBuf[0]).Buffer;
                float* vstRBuf = ((IDirectBufferAccess32)outBuf[1]).Buffer;

                fixed (float* audioBuf = &buffer[0])
                {
                    for (int i = 0, j = 0; i < halfCount; i++)
                    {
                        *(audioBuf + j++) = *(vstLBuf + i);
                        *(audioBuf + j++) = *(vstRBuf + i);
                    }
                }
            }

            inBufMgr.Dispose();
            outBufMgr.Dispose();

            return count;
        }
    }
}
