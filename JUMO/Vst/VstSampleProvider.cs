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

        public VstSampleProvider(Plugin plugin)
        {
            _cmdstub = plugin.PluginCommandStub;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            VstAudioBufferManager inBufMgr = new VstAudioBufferManager(2, count);
            VstAudioBufferManager outBufMgr = new VstAudioBufferManager(2, count);
            VstAudioBuffer[] inBuf = inBufMgr.ToArray();
            VstAudioBuffer[] outBuf = outBufMgr.ToArray();

            _cmdstub.StartProcess();
            _cmdstub.ProcessReplacing(inBuf, outBuf);
            _cmdstub.StopProcess();

            for (int i = 0; i < count; i++)
            {
                buffer[i] = outBuf[i % 2][i];
            }

            inBufMgr.Dispose();
            outBufMgr.Dispose();

            return count;
        }
    }
}
