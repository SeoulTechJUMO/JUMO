using System;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using NAudio.Wave;

namespace JUMO.Vst
{
    public class EffectPluginManager : IDisposable
    {
        public ObservableCollection<EffectPlugin> Plugins { get; } = new ObservableCollection<EffectPlugin>();

        public bool AddPlugin(MixerChannel channel, Action<Exception> onError)
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
                return false;
            }
        }

        public bool AddPlugin(string pluginPath, MixerChannel channel, Action<Exception> onError)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub(); // TODO
                EffectPlugin plugin = new EffectPlugin(pluginPath, hostCmdStub);

                Plugins.Add(plugin);

                return true;
            }
            catch (Exception e)
            {
                onError?.Invoke(e);

                return false;
            }
        }

        public void Dispose()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Dispose();
            }
        }
    }
}
