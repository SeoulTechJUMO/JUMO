using NAudio.Wave;
using System;

namespace JUMO.Mixer
{
    public class VolumePanningProvider : ISampleProvider
    {
        private readonly ISampleProvider source;

        private readonly float[] maxSamples;
        private int sampleCount;
        private readonly int channels;
        private readonly StreamVolumeEventArgs args;

        /// <summary>
        /// Number of Samples per notification
        /// </summary>
        public int SamplesPerNotification { get; set; }

        /// <summary>
        /// Raised periodically to inform the user of the max volume
        /// </summary>
        public event EventHandler<StreamVolumeEventArgs> StreamVolume;

        /// <summary>
        /// Initializes a new instance of VolumeSampleProvider
        /// </summary>
        /// <param name="source">Source Sample Provider</param>
        public VolumePanningProvider(ISampleProvider source, int samplesPerNotification)
        {
            this.source = source;
            Volume = 1.0f;
            Panning = 0.0f;
            Mute = false;

            channels = source.WaveFormat.Channels;
            maxSamples = new float[channels];
            SamplesPerNotification = samplesPerNotification;
            args = new StreamVolumeEventArgs() { MaxSampleValues = maxSamples };
        }

        public VolumePanningProvider(ISampleProvider source)
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
        /// <param name="sampleCount">Number of samples desired</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);
            float[] tempBuf = new float[sampleCount];

            if (Volume != 1f)
            {
                for (int n = 0; n < sampleCount; n++)
                {
                    if (Panning > 0)
                    {
                        if ((offset + n) % 2 == 0)
                        {
                            //왼쪽
                            tempBuf[offset + n] = buffer[offset + n] * (1 - Panning);
                        }
                        else
                        {
                            //오른쪽
                            tempBuf[offset + n] = buffer[offset + n];
                        }
                    }
                    else
                    {
                        if ((offset + n) % 2 == 0)
                        {
                            //왼쪽
                            tempBuf[offset + n] = buffer[offset + n];
                        }
                        else
                        {
                            //오른쪽
                            tempBuf[offset + n] = buffer[offset + n] * (1 - (-Panning));
                        }
                    }
                    tempBuf[offset + n] *= Volume;
                    if (Mute) { buffer[offset + n] = 0; }
                    else { buffer[offset + n] = tempBuf[offset + n]; }
                }
            }

            if (StreamVolume != null)
            {
                for (int index = 0; index < samplesRead; index += channels)
                {
                    for (int channel = 0; channel < channels; channel++)
                    {
                        float sampleValue = Math.Abs(tempBuf[offset + index + channel]);
                        maxSamples[channel] = Math.Max(maxSamples[channel], sampleValue);
                    }
                    sampleCount++;
                    if (sampleCount >= SamplesPerNotification)
                    {
                        StreamVolume(this, args);
                        sampleCount = 0;
                        // n.b. we avoid creating new instances of anything here
                        Array.Clear(maxSamples, 0, channels);
                    }
                }
            }

            return samplesRead;
        }

        /// <summary>
        /// Allows adjusting the volume, 1.0f = full volume
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
