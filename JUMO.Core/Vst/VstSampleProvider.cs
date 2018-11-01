﻿using System;
using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;

namespace JUMO.Vst
{
    class VstSampleProvider : ISampleProvider, IDisposable
    {
        private const int SAMPLE_RATE = 44100;
        private const int NUM_CHANNEL = 2;

        private readonly PluginBase _plugin;
        private readonly int _inputCount;
        private readonly int _outputCount;

        private int _totalSamples = -1;
        private VstAudioBufferManager _inBufMgr, _outBufMgr;
        private VstAudioBuffer[] _inBuf, _outBuf;
        private float[] _tempBuf;

        public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, NUM_CHANNEL);

        public VstSampleProvider(PluginBase plugin)
        {
            _plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));

            var pluginInfo = _plugin.PluginCommandStub.PluginContext.PluginInfo;

            _inputCount = pluginInfo.AudioInputCount;
            _outputCount = pluginInfo.AudioOutputCount;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            PrepareBuffers(count);

            int samplesPerBuffer = count / NUM_CHANNEL;

            if (_plugin.FetchEvents(out VstEvent[] events))
            {
                _plugin.PluginCommandStub.ProcessEvents(events);
            }

            _plugin.PluginCommandStub.ProcessReplacing(_inBuf, _outBuf);

            unsafe
            {
                int stereoOutputCount = _outputCount >> 1;

                for (int x = 0, y = 0; x < stereoOutputCount; x++)
                {
                    float* outLBuf = ((IDirectBufferAccess32)_outBuf[y++]).Buffer;
                    float* outRBuf = ((IDirectBufferAccess32)_outBuf[y++]).Buffer;

                    fixed (float* pBuf = &buffer[0])
                    {
                        if (x == 0)
                        {
                            for (int i = 0, j = 0; i < samplesPerBuffer; i++)
                            {
                                pBuf[j++] = outLBuf[i];
                                pBuf[j++] = outRBuf[i];
                            }
                        }
                        else
                        {
                            for (int i = 0, j = 0; i < samplesPerBuffer; i++)
                            {
                                pBuf[j++] += outLBuf[i];
                                pBuf[j++] += outRBuf[i];
                            }
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
            _inBufMgr = new VstAudioBufferManager(_inputCount, samplesPerBuffer);
            _outBufMgr = new VstAudioBufferManager(_outputCount, samplesPerBuffer);

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
