using Jacobi.Vst.Core.Host;
using NAudio.Wave;
using MidiToolkit = Sanford.Multimedia.Midi;
using JUMO.Mixer;

namespace JUMO.Vst
{
    public class Plugin : PluginBase
    {
        private readonly VolumePanningSampleProvider _volume;

        private int _channelNum = 0;

        public int ChannelNum
        {
            get => _channelNum;
            set
            {
                if (value < MixerManager.NumOfMixerChannels)
                {
                    MixerManager.Instance.ChangeChannel(this, value);
                    _channelNum = value;
                    OnPropertyChanged(nameof(ChannelNum));
                }
            }
        }

        public float Volume
        {
            get => _volume.Volume;
            set
            {
                _volume.Volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        public float Panning
        {
            get => _volume.Panning;
            set
            {
                _volume.Panning = value;
                OnPropertyChanged(nameof(Panning));
            }
        }

        public bool Mute
        {
            get => _volume.Mute;
            set
            {
                _volume.Mute = value;
                OnPropertyChanged(nameof(Mute));
            }
        }

        public ISampleProvider SampleProvider { get; }

        public Plugin(string pluginPath) : base(pluginPath)
        {
            _volume = new VolumePanningSampleProvider(new VstSampleProvider(this));
            Volume = 0.8f;
            Panning = 0.0f;
            Mute = false;
            SampleProvider = _volume;
            ChannelNum = 0;
        }

        public void NoteOn(byte value, byte velocity) => NoteOn(0, value, velocity);

        public void NoteOff(byte value) => NoteOff(0, value);

        public void NoteOn(int tick, byte value, byte velocity)
        {
            SendEvent(tick, new MidiToolkit.ChannelMessage(MidiToolkit.ChannelCommand.NoteOn, 0, value, velocity));
        }

        public void NoteOff(int tick, byte value)
        {
            SendEvent(tick, new MidiToolkit.ChannelMessage(MidiToolkit.ChannelCommand.NoteOff, 0, value, 64));
        }

        protected override void Dispose(bool disposing)
        {
            MixerManager.Instance.MixerChannels[ChannelNum].MixerRemoveInput(SampleProvider);
            base.Dispose(disposing);
        }
    }
}
