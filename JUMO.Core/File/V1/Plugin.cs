using System;

namespace JUMO.File.V1
{
    [Serializable]
    class Plugin
    {
        public int Id { get; }

        // From PluginBase
        public string Name { get; }
        public string PluginPath { get; }

        // From Plugin
        public int ChannelNum { get; }
        public float Volume { get; }
        public float Panning { get; }
        public bool Mute { get; }

        public int ProgramIndex { get; }
        public float[] Parameters { get; }
        public int ChunkSize { get; }
        public byte[] Chunk { get; }

        public Plugin(int id, Vst.Plugin source)
        {
            Id = id;
            Name = source.Name;
            PluginPath = source.PluginPath;
            ChannelNum = source.ChannelNum;
            Volume = source.Volume;
            Panning = source.Panning;
            Mute = source.Mute;
            source.PluginCommandStub.GetProgram();
            Parameters = source.DumpParameters();
            Chunk = source.PluginCommandStub.GetChunk(false);
            ChunkSize = Chunk.Length;
        }

        public void Restore(Vst.Plugin target)
        {
            target.Name = Name;
            target.ChannelNum = ChannelNum;
            target.Volume = Volume;
            target.Panning = Panning;
            target.Mute = Mute;

            target.PluginCommandStub.SetChunk(Chunk, false);
            target.PluginCommandStub.BeginSetProgram();
            target.PluginCommandStub.SetProgram(ProgramIndex);
            target.PluginCommandStub.EndSetProgram();
            target.LoadParameters(Parameters);
        }
    }
}
