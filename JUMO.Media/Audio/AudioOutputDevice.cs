using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace JUMO.Media.Audio
{
    public enum AudioOutputDeviceType
    {
        WaveOut,
        DirectSound,
        ASIO
    }

    public interface IAudioOutputDevice
    {
        AudioOutputDeviceType Type { get; }
        string FriendlyName { get; }
        object Identifier { get; }
    }

    public class WaveOutDevice : IAudioOutputDevice
    {
        public AudioOutputDeviceType Type => AudioOutputDeviceType.WaveOut;
        public string FriendlyName { get; private set; }
        public object Identifier { get; private set; }

        public WaveOutDevice(int id)
        {
            Identifier = id;
            FriendlyName = WaveOut.GetCapabilities(id).ProductName;
        }

        public override string ToString() => $"{Type}: {FriendlyName}";
    }

    public class DirectSoundOutputDevice : IAudioOutputDevice
    {
        public AudioOutputDeviceType Type => AudioOutputDeviceType.DirectSound;
        public string FriendlyName { get; private set; }
        public object Identifier { get; private set; }

        public DirectSoundOutputDevice(Guid guid, string description)
        {
            FriendlyName = description;
            Identifier = guid;
        }

        public override string ToString() => $"{Type}: {FriendlyName}";
    }

    public class AsioOutputDevice : IAudioOutputDevice
    {
        public AudioOutputDeviceType Type => AudioOutputDeviceType.ASIO;
        public string FriendlyName { get; private set; }
        public object Identifier => FriendlyName;

        public AsioOutputDevice(string driverName)
        {
            FriendlyName = driverName;
        }

        public override string ToString() => $"{Type}: {FriendlyName}";
    }
}
