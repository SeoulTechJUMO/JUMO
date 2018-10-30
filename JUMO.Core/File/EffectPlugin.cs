using System;

namespace JUMO.File
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

        public EffectPlugin(Vst.EffectPlugin source)
        {
            Name = source.Name;
            PluginPath = source.PluginPath;
            EffectMix = source.EffectMix;
            Parameters = source.DumpParameters();
        }
    }
}
