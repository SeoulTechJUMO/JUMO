using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
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
        private readonly BlockingCollection<VstEvent[]> _bc = new BlockingCollection<VstEvent[]>();

        private bool _isDisposed = false;
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

            new Thread(WorkerMethod) { Name = $"Worker thread for VST plugin {Name}" }.Start();
        }

        public void NoteOn(byte value, byte velocity)
        {
            _bc.Add(new VstEvent[]
            {
                new VstMidiEvent(0, 0, 0, new byte[] { 0x90, value, velocity, 0x00 }, 0, 64)
            });
        }

        public void NoteOff(byte value)
        {
            _bc.Add(new VstEvent[]
            {
                new VstMidiEvent(0, 0, 0, new byte[] { 0x80, value, 64, 0x00 }, 0, 64)
            });
        }

        public void SendEvents(VstEvent[] events) => _bc.Add(events);

        private void WorkerMethod()
        {
            while (!_bc.IsCompleted)
            {
                VstEvent[] nextEvents = null;

                try
                {
                    nextEvents = _bc.Take();
                }
                catch (InvalidOperationException) { }

                if (nextEvents != null)
                {
                    PluginCommandStub.StartProcess();
                    PluginCommandStub.ProcessEvents(nextEvents);
                    PluginCommandStub.StopProcess();
                }
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
                _bc.CompleteAdding();
                _bc.Dispose();
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
