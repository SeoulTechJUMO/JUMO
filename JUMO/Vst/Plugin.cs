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
        private readonly IVstPluginCommandStub _cmdstub;

        public string Name { get; set; }
        public string PluginPath { get; }
        public ISampleProvider SampleProvider { get; }

        public Plugin(string pluginPath, IVstHostCommandStub hostCmdStub)
        {
            PluginPath = pluginPath;

            _ctx = VstPluginContext.Create(PluginPath, hostCmdStub);
            _cmdstub = _ctx.PluginCommandStub;
            _cmdstub.Open();
            _cmdstub.SetSampleRate(44100.0f);
            _cmdstub.SetBlockSize(2048);
            _cmdstub.MainsChanged(true);

            Name = _cmdstub.GetEffectName();
            SampleProvider = new VstSampleProvider(_cmdstub);
        }

        public void Dispose()
        {
            _cmdstub.MainsChanged(false);
            _cmdstub.Close();
        }
    }
}
