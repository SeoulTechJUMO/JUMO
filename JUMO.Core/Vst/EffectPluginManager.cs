using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace JUMO.Vst
{
    public class EffectPluginManager : IDisposable
    {
        public ObservableCollection<EffectPlugin> Plugins { get; } = new ObservableCollection<EffectPlugin>();

        public EffectPlugin AddPlugin(string pluginPath, MixerChannel channel, Action<Exception> onError)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub(); // TODO
                EffectPlugin plugin = new EffectPlugin(pluginPath, hostCmdStub);

                lock (((ICollection)Plugins).SyncRoot)
                {
                    Plugins.Add(plugin);
                }

                return plugin;
            }
            catch (Exception e)
            {
                onError?.Invoke(e);

                return null;
            }
        }

        public EffectPlugin ReplacePlugin(string pluginPath, MixerChannel channel, Action<Exception> onError, EffectPlugin oldPlugin)
        {
            int idx = Plugins.IndexOf(oldPlugin ?? throw new ArgumentNullException(nameof(oldPlugin)));

            try
            {
                EffectPlugin plugin = new EffectPlugin(pluginPath, new HostCommandStub())
                {
                    EffectMix = oldPlugin.EffectMix
                };

                RemovePlugin(oldPlugin);

                lock (((ICollection)Plugins).SyncRoot)
                {
                    Plugins.Insert(idx, plugin);
                }

                return plugin;
            }
            catch (Exception e)
            {
                onError?.Invoke(e);

                return null;
            }
        }

        public void MoveUp(int idx)
        {
            lock (((ICollection)Plugins).SyncRoot)
            {
                if (idx == 0)
                {
                    Plugins.Move(idx, Plugins.Count - 1);
                }
                else
                {
                    Plugins.Move(idx, idx - 1);
                }
            }
        }

        public void MoveDown(int idx)
        {
            lock (((ICollection)Plugins).SyncRoot)
            {
                if (idx == Plugins.Count - 1)
                {
                    Plugins.Move(idx, 0);
                }
                else
                {
                    Plugins.Move(idx, idx + 1);
                }
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
