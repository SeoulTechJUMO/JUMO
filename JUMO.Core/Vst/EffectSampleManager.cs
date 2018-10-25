using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace JUMO.Vst
{
    public class EffectSampleManager : ISampleProvider
    {
        public EffectSampleManager(ISampleProvider source)
        {
            CurrentOut = source;
        }

        public ISampleProvider CurrentOut { get; set; }

        public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            return sampleCount;
        }
    }
}
