using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;

namespace JUMO.Vst
{
    public class Plugin : IDisposable
    {
        private readonly IVstPluginContext _ctx;

        public string Name { get; set; }
        public string PluginPath { get; }
        public IVstPluginCommandStub PluginCommandStub { get; }
        public ISampleProvider SampleProvider { get; }

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
            SampleProvider = new VstSampleProvider(this);
        }

        public void Dispose()
        {
            PluginCommandStub.MainsChanged(false);
            PluginCommandStub.Close();
        }
    }
}
