using System;
using System.Collections.ObjectModel;

namespace JUMO.Vst
{
    public sealed class PluginManager : IDisposable
    {
        #region Singleton

        private static readonly Lazy<PluginManager> _instance = new Lazy<PluginManager>(() => new PluginManager());

        public static PluginManager Instance => _instance.Value;

        private PluginManager() { }

        #endregion

        public ObservableCollection<Plugin> Plugins { get; } = new ObservableCollection<Plugin>();

        public Plugin AddPlugin(string pluginPath, Action<Exception> onError)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub(); // TODO
                Plugin plugin = new Plugin(pluginPath, hostCmdStub);

                Plugins.Add(plugin);

                return plugin;
            }
            catch (Exception e)
            {
                onError?.Invoke(e);

                return null;
            }
        }

        public Plugin ReplacePlugin(string pluginPath, Action<Exception> onError, Plugin oldPlugin)
        {
            int idx = Plugins.IndexOf(oldPlugin ?? throw new ArgumentNullException(nameof(oldPlugin)));

            try
            {
                Plugin plugin = new Plugin(pluginPath, new HostCommandStub())
                {
                    ChannelNum = oldPlugin.ChannelNum,
                    Volume = oldPlugin.Volume,
                    Panning = oldPlugin.Panning,
                    Mute = oldPlugin.Mute
                };

                Plugins[idx] = plugin;

                oldPlugin?.Dispose();

                return plugin;
            }
            catch (Exception e)
            {
                onError?.Invoke(e);

                return null;
            }
        }

        public void RemovePlugin(Plugin plugin)
        {
            Plugins.Remove(plugin);
            plugin?.Dispose();
        }

        public void UnloadAll()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Dispose();
            }

            Plugins.Clear();
        }

        public void Dispose()
        {
            UnloadAll();
        }
    }
}
