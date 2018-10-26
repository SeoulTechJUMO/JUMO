using System;
using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;

namespace JUMO.Vst
{
    class VstSampleProvider : ISampleProvider
    {
        private const int SAMPLE_RATE = 44100;
        private const int NUM_CHANNEL = 2;

        private readonly Plugin _plugin;

        private int _totalSamples = -1;
        private VstAudioBufferManager _inBufMgr, _outBufMgr;
        private VstAudioBuffer[] _inBuf, _outBuf;
        private float[] _tempBuf;

        public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, NUM_CHANNEL);

        public ISampleProvider Source { get; set; }
        public float EffectMix { get; set; }

        public VstSampleProvider(Plugin plugin) : this(plugin, null) { }

        public VstSampleProvider(Plugin plugin, ISampleProvider source)
        {
            _plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            Source = source;
            EffectMix = 1.0f;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            PrepareBuffers(count);

            int samplesPerBuffer = count / NUM_CHANNEL;

            if (_plugin.FetchEvents(out VstEvent[] events))
            {
                _plugin.PluginCommandStub.ProcessEvents(events);
            }

            if (Source != null)
            {
                Source.Read(_tempBuf, offset, count);

                unsafe
                {
                    float* inLBuf = ((IDirectBufferAccess32)_inBuf[0]).Buffer;
                    float* inRBuf = ((IDirectBufferAccess32)_inBuf[1]).Buffer;

                    fixed (float* pTempBuf = &_tempBuf[0])
                    {
                        for (int i = 0, j = 0; i < samplesPerBuffer; i++)
                        {
                            inLBuf[i] = pTempBuf[j++];
                            inRBuf[i] = pTempBuf[j++];
                        }
                    }
                }
            }

            _plugin.PluginCommandStub.ProcessReplacing(_inBuf, _outBuf);

            unsafe
            {
                float* vstLBuf = ((IDirectBufferAccess32)_outBuf[0]).Buffer;
                float* vstRBuf = ((IDirectBufferAccess32)_outBuf[1]).Buffer;

                fixed (float* audioBuf = &buffer[0], pTempBuf = &_tempBuf[0])
                {
                    for (int i = 0, j = 0; i < samplesPerBuffer; i++)
                    {
                        if (Source != null)
                        {
                            audioBuf[j] = vstLBuf[i] * EffectMix + pTempBuf[j] * (1 - EffectMix); j++;
                            audioBuf[j] = vstRBuf[i] * EffectMix + pTempBuf[j] * (1 - EffectMix); j++;
                        }
                        else
                        {
                            audioBuf[j++] = vstLBuf[i];
                            audioBuf[j++] = vstRBuf[i];
                        }
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

            System.Diagnostics.Debug.WriteLine($"VstSampleProvider::PrepareBuffers [{_plugin.PluginCommandStub.GetEffectName()}] count changed from {_totalSamples} to {count}. Replacing current buffers.");

            _inBufMgr?.Dispose();
            _outBufMgr?.Dispose();

            int samplesPerBuffer = count / NUM_CHANNEL;

            _tempBuf = new float[count];
            _inBufMgr = new VstAudioBufferManager(2, samplesPerBuffer);
            _outBufMgr = new VstAudioBufferManager(2, samplesPerBuffer);

#pragma warning disable CS0618 // Type or member is obsolete
            _inBuf = _inBufMgr.ToArray();
            _outBuf = _outBufMgr.ToArray();
#pragma warning restore CS0618 // Type or member is obsolete

            _totalSamples = count;
        }
    }
}
