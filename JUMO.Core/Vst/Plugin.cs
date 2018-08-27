using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace JUMO.Vst
{
    public class Plugin : IDisposable, INotifyPropertyChanged
    {
        private readonly IVstPluginContext _ctx;
        private readonly VolumeSampleProvider _volume;
        private string _name;

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

            Name = PluginCommandStub.GetEffectName();
            _volume = new VolumeSampleProvider(new VstSampleProvider(this));
            SampleProvider = _volume;
        }

        public void NoteOn(byte value, byte velocity)
        {
            PluginCommandStub.StartProcess();
            PluginCommandStub.ProcessEvents(new VstEvent[]
            {
                new VstMidiEvent(0, 0, 0, new byte[] { 0x90, value, velocity, 0x00 }, 0, velocity)
            });
            PluginCommandStub.StopProcess();
        }

        public void NoteOff(byte value, byte velocity)
        {
            PluginCommandStub.StartProcess();
            PluginCommandStub.ProcessEvents(new VstEvent[]
            {
                new VstMidiEvent(0, 0, 0, new byte[] { 0x80, value, velocity, 0x00 }, 0, velocity)
            });
            PluginCommandStub.StopProcess();
        }

        public void Dispose()
        {
            PluginCommandStub.MainsChanged(false);
            PluginCommandStub.Close();
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
