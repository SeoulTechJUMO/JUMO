using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace JUMO.Vst
{
    public class EffectPluginManager : IDisposable
    {
        public ObservableCollection<EffectPlugin> Plugins { get; } = new ObservableCollection<EffectPlugin>();

        public EffectPlugin AddPlugin(string pluginPath, MixerChannel channel, Action<Exception> onError, bool replace = false, EffectPlugin oldPlugin = null)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub(); // TODO
                EffectPlugin plugin = new EffectPlugin(pluginPath, hostCmdStub);

                if(replace)
                {
                    int idx = Plugins.IndexOf(oldPlugin);
                    RemovePlugin(oldPlugin);
                    Plugins.Insert(idx, plugin);
                }
                else
                {
                    lock (((ICollection)Plugins).SyncRoot)
                    {
                        Plugins.Add(plugin);
                    }
                }

                return plugin;
            }
            catch (Exception e)
            {
                onError?.Invoke(e);

                return null;
            }
        }

        public void RemovePlugin(EffectPlugin plugin)
        {
            lock (((ICollection)Plugins).SyncRoot)
            {
                Plugins.Remove(plugin);
            }

            plugin.Dispose();
        }

        public void UnloadAll()
        {
            lock (((ICollection)Plugins).SyncRoot)
            {
                foreach (EffectPlugin plugin in Plugins)
                {
                    plugin.Dispose();
                }

                Plugins.Clear();
            }
        }

        public void Dispose()
        {
            UnloadAll();
        }
    }
}
