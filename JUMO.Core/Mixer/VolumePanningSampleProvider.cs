using NAudio.Wave;
using System;

namespace JUMO.Mixer
{
    public class VolumePanningSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;

        private readonly float[] _maxSamples;
        private readonly int _channels;
        private readonly StreamVolumeEventArgs _eventArgs;
        private int _lastTempBufSize = -1;
        private int _sampleCount = 0;
        private float[] _tempBuf;

        #region Properties

        /// <summary>
        /// 샘플 확인 주기
        /// </summary>
        public int SamplesPerNotification { get; set; }

        /// <summary>
        /// 볼륨 값, 1.0f = full volume
        /// </summary>
        public float Volume { get; set; }

        /// <summary>
        /// panning정보 0이 센터
        /// </summary>
        public float Panning { get; set; }

        /// <summary>
        /// 뮤트
        /// </summary>
        public bool Mute { get; set; }

        #endregion

        /// <summary>
        /// 이벤트 발생시에 최대 볼륨 값을 알려주는 이벤트
        /// </summary>
        public event EventHandler<StreamVolumeEventArgs> StreamVolume;

        /// <summary>
        /// 믹서 채널용 생성자
        /// </summary>
        /// <param name="source">Source Sample Provider</param>
        public VolumePanningSampleProvider(ISampleProvider source, int samplesPerNotification)
        {
            this.source = source;
            Volume = 1.0f;
            Panning = 0.0f;
            Mute = false;

            _channels = source.WaveFormat.Channels;
            _maxSamples = new float[_channels];
            SamplesPerNotification = samplesPerNotification;
            _eventArgs = new StreamVolumeEventArgs() { MaxSampleValues = _maxSamples };
        }

        /// <summary>
        /// 플러그인용 생성자
        /// </summary>
        /// <param name="source"></param>
        public VolumePanningSampleProvider(ISampleProvider source)
        {
            this.source = source;
            Volume = 1.0f;
            Panning = 0.0f;
            Mute = false;
        }

        /// <summary>
        /// WaveFormat
        /// </summary>
        public WaveFormat WaveFormat => source.WaveFormat;

        /// <summary>
        /// Reads samples from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="count">Number of samples desired</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);

            if (samplesRead != _lastTempBufSize)
            {
                _tempBuf = new float[samplesRead];
                _lastTempBufSize = samplesRead;
            }

            unsafe
            {
                fixed (float* pTempBuf = &_tempBuf[0], pBuf = &buffer[0])
                {
                    for (int n = 0; n < samplesRead; n++)
                    {
                        int index = offset + n;

                        if (Panning > 0)
                        {
                            pTempBuf[index] = pBuf[index] * (index % 2 == 0 ? 1 - Panning : 1);
                        }
                        else
                        {
                            pTempBuf[index] = pBuf[index] * (index % 2 != 0 ? 1 - (-Panning) : 1);
                        }

                        pTempBuf[index] *= Volume;
                        pBuf[index] = Mute ? 0 : pTempBuf[index];
                    }
                }
            }

            if (StreamVolume != null)
            {
                unsafe
                {
                    fixed (float *pTempBuf = &_tempBuf[0], pMaxSamples = &_maxSamples[0])
                    {
                        for (int index = 0; index < samplesRead; index += _channels)
                        {
                            for (int channel = 0; channel < _channels; channel++)
                            {
                                float sampleValue = Math.Abs(pTempBuf[offset + index + channel]);
                                pMaxSamples[channel] = Math.Max(pMaxSamples[channel], sampleValue);
                            }

                            _sampleCount++;

                            if (_sampleCount >= SamplesPerNotification)
                            {
                                StreamVolume(this, _eventArgs);

                                _sampleCount = 0;

                                // n.b. we avoid creating new instances of anything here
                                Array.Clear(_maxSamples, 0, _channels);
                            }
                        }
                    }
                }
            }

            return samplesRead;
        }

        /// <summary>
        /// Event args for aggregated stream volume
        /// </summary>
        public class StreamVolumeEventArgs : EventArgs
        {
            /// <summary>
            /// Max sample values array (one for each channel)
            /// </summary>
            public float[] MaxSampleValues { get; set; }
        }
    }
}
