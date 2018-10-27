using System;
using System.Collections.Generic;
using System.ComponentModel;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using MidiToolkit = Sanford.Multimedia.Midi;

namespace JUMO.Vst
{
    public abstract class PluginBase : IDisposable, INotifyPropertyChanged
    {
        private readonly IVstPluginContext _ctx;
        private readonly List<VstMidiEvent> _pendingEvents = new List<VstMidiEvent>();

        private readonly object _lock = new object();

        private bool _isDisposed = false;
        private string _name;

        #region Properties

        protected int FirstTick { get; private set; } = -1;

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

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public PluginBase(string pluginPath, IVstHostCommandStub hostCmdStub)
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
        }

        public void SendEvent(int tick, MidiToolkit.IMidiMessage msg)
        {
            lock (_lock)
            {
                if (FirstTick == -1)
                {
                    FirstTick = tick;
                }

                int deltaFrames = (int)(44100 * (tick - FirstTick) * Song.Current.SecondsPerTick);

                _pendingEvents.Add(new VstMidiEvent(Math.Max(0, deltaFrames), 0, 0, msg.GetBytes(), 0, 0));
            }
        }

        public bool FetchEvents(out VstEvent[] events)
        {
            lock (_lock)
            {
                FirstTick = -1;

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

        protected void OnPropertyChanged(string propertyName)
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
