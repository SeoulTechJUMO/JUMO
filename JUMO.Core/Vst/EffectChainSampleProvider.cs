using System;
using System.Collections.Generic;
using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;

namespace JUMO.Vst
{
    class EffectChainSampleProvider : ISampleProvider, IDisposable
    {
        private const int SAMPLE_RATE = 44100;
        private const int NUM_CHANNEL = 2;

        private readonly ISampleProvider _source;
        private readonly IList<EffectPlugin> _plugins;

        private int _totalSamples = -1;
        private VstAudioBufferManager _inBufMgr, _outBufMgr;
        private VstAudioBuffer[] _inBuf, _outBuf;
        private float[] _tempBuf;

        public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, NUM_CHANNEL);

        public EffectChainSampleProvider(ISampleProvider source, IList<EffectPlugin> plugins)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _plugins = plugins ?? throw new ArgumentNullException(nameof(plugins));
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (_plugins.Count == 0)
            {
                return _source.Read(buffer, offset, count);
            }

            PrepareBuffers(count);

            int samplesPerBuffer = count / NUM_CHANNEL;

            _source.Read(_tempBuf, offset, count);

            unsafe
            {
                float* inLBuf = ((IDirectBufferAccess32)_inBuf[0]).Buffer;
                float* inRBuf = ((IDirectBufferAccess32)_inBuf[1]).Buffer;

                fixed (float *pTempBuf = &_tempBuf[0])
                {
                    for (int i = 0, j = 0; i < samplesPerBuffer; i++)
                    {
                        inLBuf[i] = pTempBuf[j++];
                        inRBuf[i] = pTempBuf[j++];
                    }
                }
            }

            VstAudioBuffer[] swapTemp;

            for (int i = 0; i < _plugins.Count; i++)
            {
                _plugins[i].ProcessEffect(_inBuf, _outBuf, samplesPerBuffer);

                swapTemp = _inBuf;
                _inBuf = _outBuf;
                _outBuf = swapTemp;
            }

            unsafe
            {
                float* outLBuf = ((IDirectBufferAccess32)_inBuf[0]).Buffer;
                float* outRBuf = ((IDirectBufferAccess32)_inBuf[1]).Buffer;

                fixed (float *audioBuf = &buffer[0])
                {
                    for (int i = 0, j = 0; i < samplesPerBuffer; i++)
                    {
                        audioBuf[j++] = outLBuf[i];
                        audioBuf[j++] = outRBuf[i];
                    }
                }
            }

            return count;
        }

        private void PrepareBuffers(int count)
        {
            if (count == _totalSamples)
            {
                return;
            }

            _inBufMgr?.Dispose();
            _outBufMgr?.Dispose();

            int samplesPerBuffer = count / NUM_CHANNEL;

            _tempBuf = new float[count];
            _inBufMgr = new VstAudioBufferManager(NUM_CHANNEL, samplesPerBuffer);
            _outBufMgr = new VstAudioBufferManager(NUM_CHANNEL, samplesPerBuffer);

#pragma warning disable CS0618 // Type or member is obsolete
            _inBuf = _inBufMgr.ToArray();
            _outBuf = _outBufMgr.ToArray();
#pragma warning restore CS0618 // Type or member is obsolete

            _totalSamples = count;
        }

        public void Dispose()
        {
            _inBufMgr?.Dispose();
            _outBufMgr?.Dispose();
        }
    }
}
