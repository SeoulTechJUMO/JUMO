using System;
using System.Collections.Generic;
using System.ComponentModel;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using MidiToolkit = Sanford.Multimedia.Midi;

namespace JUMO.Vst
{
    public class Plugin : IDisposable, INotifyPropertyChanged
    {
        private readonly IVstPluginContext _ctx;
        private readonly VolumeSampleProvider _volume;
        private readonly List<VstMidiEvent> _pendingEvents = new List<VstMidiEvent>();

        private readonly object _lock = new object();

        private bool _isDisposed = false;
        private string _name;
        private int _firstTick = -1;
        private int _ChannelNum = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        // TODO: NAudio에서 제공하는 PanningSampleProvider는 Mono to Stereo 전용.
        //       Stereo to Stereo를 별도로 구현해야 함.
        // private readonly PanningSampleProvider _pan;

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
            get => _ChannelNum;
            set
            {
                MixerManager.Instance.ChangeChannel(this, value);

                _ChannelNum = value;
                OnPropertyChanged(nameof(ChannelNum));
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

            if (source != null)
            {
                _volume = new VolumeSampleProvider(new VstSampleProvider(this, source));
            }
            else
            {
                _volume = new VolumeSampleProvider(new VstSampleProvider(this));
            }

            SampleProvider = _volume;
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
