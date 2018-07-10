using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace JUMO.Media.Audio
{
    public class AudioOutputEngine : IDisposable
    {
        private const int SAMPLE_RATE = 44100;
        private const int NUM_CHANNEL = 2;

        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        public AudioOutputEngine(IAudioOutputDevice device)
        {
            System.Diagnostics.Debug.WriteLine($"Initializing a new audio output engine with {device.FriendlyName}");
            switch (device.Type)
            {
                case AudioOutputDeviceType.WaveOut:
                    outputDevice = new WaveOut() { DeviceNumber = (int)device.Identifier };
                    break;
                case AudioOutputDeviceType.DirectSound:
                    outputDevice = new DirectSoundOut((Guid)device.Identifier);
                    break;
                case AudioOutputDeviceType.ASIO:
                    outputDevice = new AsioOut((string)device.Identifier);
                    break;
            }

            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, NUM_CHANNEL))
            {
                ReadFully = true
            };

            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        public void AddMixerInput(ISampleProvider input)
        {
            mixer.AddMixerInput(input);
        }

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("Disposing the current audio output engine");
            outputDevice.Stop();
            outputDevice.Dispose();
        }
    }
}
