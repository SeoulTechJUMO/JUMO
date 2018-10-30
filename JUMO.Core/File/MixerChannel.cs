using System;

namespace JUMO.File
{
    [Serializable]
    class MixerChannel
    {
        public float Panning { get; }
        public double Volume { get; }
        public bool IsMuted { get; }
        public EffectPlugin[] Plugins { get; }

        public MixerChannel(JUMO.MixerChannel source)
        {
            Panning = source.Panning;
            Volume = source.Volume;
            IsMuted = source.IsMuted;

            Plugins = new EffectPlugin[source.Plugins.Count];
            int pluginIndex = 0;

            foreach (Vst.EffectPlugin plugin in source.Plugins)
            {
                Plugins[pluginIndex++] = new EffectPlugin(plugin);
            }
        }
    }
}
