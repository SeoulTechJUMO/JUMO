﻿using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;

namespace VstHostTest
{
    class VSTSampleProvider : ISampleProvider
    {
        private readonly IVstPluginCommandStub _cmdstub;

        public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        public VSTSampleProvider(IVstPluginCommandStub stub)
        {
            _cmdstub = stub;
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
                buffer[i] = outBuf[0][i];
            }

            return count;
        }
    }
}
