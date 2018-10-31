using System;

namespace JUMO.File
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

        public float[] Parameters { get; }

        public Plugin(int id, Vst.Plugin source)
        {
            Id = id;
            Name = source.Name;
            PluginPath = source.PluginPath;
            ChannelNum = source.ChannelNum;
            Volume = source.Volume;
            Panning = source.Panning;
            Mute = source.Mute;
            Parameters = source.DumpParameters();
        }

        public void Restore(Vst.Plugin target)
        {
            target.Name = Name;
            target.ChannelNum = ChannelNum;
            target.Volume = Volume;
            target.Panning = Panning;
            target.Mute = Mute;

            target.LoadParameters(Parameters);
        }
    }
}
