using Jacobi.Vst.Core;
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

        public void Restore(Vst.Plugin target)
        {
            target.Name = Name;
            target.ChannelNum = ChannelNum;
            target.Volume = Volume;
            target.Panning = Panning;
            target.Mute = Mute;

            if (target.PluginCommandStub.PluginContext.PluginInfo.Flags.HasFlag(VstPluginFlags.ProgramChunks))
            {
                target.PluginCommandStub.SetChunk(Chunk, false);
            }

            target.PluginCommandStub.BeginSetProgram();
            target.PluginCommandStub.SetProgram(ProgramIndex);
            target.PluginCommandStub.EndSetProgram();
            target.LoadParameters(Parameters);
        }
    }
}
