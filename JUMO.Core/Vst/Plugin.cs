using System;
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
        private readonly object _lock = new object();

        private bool _isDisposed = false;
        private string _name;
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
            PluginCommandStub.SetBlockSize(2048);
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

        public void NoteOn(byte value, byte velocity)
        {
            lock (_lock)
            {
                PluginCommandStub.ProcessEvents(new[] { new VstMidiEvent(0, 0, 0, new byte[] { 0x90, value, velocity, 0x00 }, 0, 64) });
            }
        }

        public void NoteOff(byte value)
        {
            lock (_lock)
            {
                PluginCommandStub.ProcessEvents(new[] { new VstMidiEvent(0, 0, 0, new byte[] { 0x80, value, 64, 0x00 }, 0, 64) });
            }
        }

        public void SendEvents(VstEvent[] events)
        {
            lock (_lock)
            {
                PluginCommandStub.ProcessEvents(events);
            }
        }

        public void SendEvent(VstEvent msg)
        {
            lock (_lock)
            {
                PluginCommandStub.ProcessEvents(new[] { msg });
            }
        }

        public void SendEvent(MidiToolkit.IMidiMessage msg)
        {
            lock (_lock)
            {
                PluginCommandStub.ProcessEvents(new[] { new VstMidiEvent(0, 0, 0, msg.GetBytes(), 0, 64) });
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
