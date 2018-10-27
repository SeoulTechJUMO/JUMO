using System;
using System.Collections.Generic;
using System.ComponentModel;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using MidiToolkit = Sanford.Multimedia.Midi;
using JUMO.Mixer;

namespace JUMO.Vst
{
    public class Plugin : IDisposable, INotifyPropertyChanged
    {
        private readonly IVstPluginContext _ctx;
        private readonly List<VstMidiEvent> _pendingEvents = new List<VstMidiEvent>();
        private readonly VolumePanningSampleProvider _volume;

        private readonly object _lock = new object();

        private bool _isDisposed = false;
        private string _name;
        private int _firstTick = -1;
        private int _channelNum = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string PluginPath { get; }
        public IVstPluginCommandStub PluginCommandStub { get; }
        public ISampleProvider SampleProvider { get; }

        //input오디오 소스
        public ISampleProvider source;

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

        public float EffectMix
        {
            get => VstSample.EffectMix;
            set
            {
                VstSample.EffectMix = value;
                OnPropertyChanged(nameof(EffectMix));
            }
        }

        private VstSampleProvider VstSample { get; set; }

        public Plugin(string pluginPath, IVstHostCommandStub hostCmdStub)
        {
            PluginPath = pluginPath;

            _ctx = VstPluginContext.Create(PluginPath, hostCmdStub);
            PluginCommandStub = _ctx.PluginCommandStub;
            PluginCommandStub.Open();
            PluginCommandStub.SetSampleRate(44100.0f);
            PluginCommandStub.SetBlockSize(256);
            PluginCommandStub.MainsChanged(true);
            PluginCommandStub.StartProcess();

            Name = PluginCommandStub.GetEffectName();
            _volume = new VolumePanningSampleProvider(new VstSampleProvider(this));
            Volume = 0.8f;
            Panning = 0.0f;
            Mute = false;
            SampleProvider = _volume;
        }

        public Plugin(string pluginPath, IVstHostCommandStub hostCmdStub, ISampleProvider source)
        {
            PluginPath = pluginPath;

            _ctx = VstPluginContext.Create(PluginPath, hostCmdStub);
            PluginCommandStub = _ctx.PluginCommandStub;
            PluginCommandStub.Open();
            PluginCommandStub.SetSampleRate(44100.0f);
            PluginCommandStub.SetBlockSize(256);
            PluginCommandStub.MainsChanged(true);
            PluginCommandStub.StartProcess();

            Name = PluginCommandStub.GetEffectName();
            VstSample = new VstSampleProvider(this, source);
            SampleProvider = VstSample;
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

        public void SendEvent(int tick, MidiToolkit.IMidiMessage msg)
        {
            lock (_lock)
            {
                if (_firstTick == -1)
                {
                    _firstTick = tick;
                }

                int deltaFrames = (int)(44100 * (tick - _firstTick) * Song.Current.SecondsPerTick);

                _pendingEvents.Add(new VstMidiEvent(Math.Max(0, deltaFrames), 0, 0, msg.GetBytes(), 0, 0));
            }
        }

        public bool FetchEvents(out VstEvent[] events)
        {
            lock (_lock)
            {
                _firstTick = -1;

                if (_pendingEvents.Count == 0)
                {
                    events = null;

                    return false;
                }

                events = _pendingEvents.ToArray();

                _pendingEvents.Clear();

                return true;
            }
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                PluginCommandStub.StopProcess();
                PluginCommandStub.MainsChanged(false);
                PluginCommandStub.Close();
            }

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
