using Jacobi.Vst.Core;
using System;

namespace JUMO.File.V1
{
    [Serializable]
    class EffectPlugin
    {
        // From PluginBase
        public string Name { get; }
        public string PluginPath { get; }

        // From EffectPlugin
        public float EffectMix { get; }

        public float[] Parameters { get; }
        public int ChunkSize { get; }
        public byte[] Chunk { get; }

        public EffectPlugin(Vst.EffectPlugin source)
        {
            Name = source.Name;
            PluginPath = source.PluginPath;
            EffectMix = source.EffectMix;
            Parameters = source.DumpParameters();

            if (source.PluginCommandStub.PluginContext.PluginInfo.Flags.HasFlag(VstPluginFlags.ProgramChunks))
            {
                Chunk = source.PluginCommandStub.GetChunk(false);
                ChunkSize = Chunk.Length;
            }
            else
            {
                Chunk = new byte[0];
                ChunkSize = 0;
            }
        }

        public void Restore(Vst.EffectPlugin target)
        {
            target.Name = Name;
            target.EffectMix = EffectMix;

            if (target.PluginCommandStub.PluginContext.PluginInfo.Flags.HasFlag(VstPluginFlags.ProgramChunks))
            {
                target.PluginCommandStub.SetChunk(Chunk, false);
            }

            target.LoadParameters(Parameters);
        }
    }
}
