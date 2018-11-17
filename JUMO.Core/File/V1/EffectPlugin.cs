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
            Chunk = source.PluginCommandStub.GetChunk(false);
            ChunkSize = Chunk.Length;
        }

        public void Restore(Vst.EffectPlugin target)
        {
            target.Name = Name;
            target.EffectMix = EffectMix;

            target.PluginCommandStub.SetChunk(Chunk, false);
            target.LoadParameters(Parameters);
        }
    }
}
