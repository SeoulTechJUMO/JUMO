using System;
using NAudio.Wave;

namespace JUMO.Audio
{
    public enum AudioOutputDeviceType
    {
        WaveOut,
        DirectSound,
        Asio
    }

    public abstract class AudioOutputDevice
    {
        public abstract AudioOutputDeviceType Type { get; }
        public string FriendlyName { get; protected set; }
        public object Identifier { get; protected set; }

        public override string ToString() => $"{Type}: {FriendlyName}";
    }

    public class WaveOutDevice : AudioOutputDevice
    {
        public override AudioOutputDeviceType Type => AudioOutputDeviceType.WaveOut;

        public WaveOutDevice(int id)
        {
            Identifier = id;
            FriendlyName = WaveOut.GetCapabilities(id).ProductName;
        }
    }

    public class DirectSoundOutputDevice : AudioOutputDevice
    {
        public override AudioOutputDeviceType Type => AudioOutputDeviceType.DirectSound;

        public DirectSoundOutputDevice(Guid guid, string description)
        {
            Identifier = guid;
            FriendlyName = description;
        }
    }

    public class AsioOutputDevice : AudioOutputDevice
    {
        public override AudioOutputDeviceType Type => AudioOutputDeviceType.Asio;

        public AsioOutputDevice(string driverName)
        {
            Identifier = driverName;
            FriendlyName = driverName;
        }
    }
}
