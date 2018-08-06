using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace JUMO.Vst
{
    public class Plugin : IDisposable
    {
        private readonly IVstPluginContext _ctx;
        private readonly VolumeSampleProvider _volume;
        // TODO: NAudio에서 제공하는 PanningSampleProvider는 Mono to Stereo 전용.
        //       Stereo to Stereo를 별도로 구현해야 함.
        // private readonly PanningSampleProvider _pan;

        public string Name { get; set; }
        public string PluginPath { get; }
        public IVstPluginCommandStub PluginCommandStub { get; }
        public ISampleProvider SampleProvider { get; }

        public float Volume
        {
            get => _volume.Volume;
            set => _volume.Volume = value;
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

        public void Dispose()
        {
            PluginCommandStub.MainsChanged(false);
            PluginCommandStub.Close();
        }
    }
}
