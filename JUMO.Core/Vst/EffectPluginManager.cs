using System;
using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using NAudio.Wave;

namespace JUMO.Vst
{
    public class EffectPluginManager : IDisposable
    {
        public ObservableCollection<EffectPlugin> Plugins { get; } = new ObservableCollection<EffectPlugin>();

        public EffectPlugin AddPlugin(MixerChannel channel, Action<Exception> onError)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "VST 플러그인 (*.dll)|*.dll"
            };

            if (dlg.ShowDialog() == true)
            {
                return AddPlugin(dlg.FileName, channel, onError);
            }
            else
            {
                return null;
            }
        }

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
