using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Win32;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using JUMO.Audio;
using NAudio.Wave;

namespace JUMO.Vst
{
    public sealed class PluginManager : IDisposable
    {
        #region Singleton

        private static readonly Lazy<PluginManager> _instance = new Lazy<PluginManager>(() => new PluginManager());
        public static PluginManager Instance => _instance.Value;

        #endregion

        public ObservableCollection<Plugin> Plugins { get; } = new ObservableCollection<Plugin>();

        public bool AddPlugin(Action<Exception> onError)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "VST 플러그인 (*.dll)|*.dll"
            };

            if (dlg.ShowDialog() == true)
            {
                return AddPlugin(dlg.FileName, onError);
            }
            else
            {
                return false;
            }
        }

        public bool AddPlugin(string pluginPath, Action<Exception> onError)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub(); // TODO
                Plugin plugin = new Plugin(pluginPath, hostCmdStub);

                MixerManager.Instance.MixerChannels[plugin.ChannelNum].MixerSendInput(plugin.SampleProvider);

                Plugins.Add(plugin);

                return true;
            }
            catch (Exception e)
            {
                onError?.Invoke(e);

                return false;
            }
        }

        public void PluginDispose(Plugin plugin)
        {
            plugin.Dispose();
        }

        public void Dispose()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Dispose();
            }
        }
    }

    public class EffectPluginManager : IDisposable
    {
        public ObservableCollection<Plugin> Plugins { get; } = new ObservableCollection<Plugin>();

        public bool AddPlugin(MixerChannel channel, ISampleProvider source, Action<Exception> onError)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "VST 플러그인 (*.dll)|*.dll"
            };

            if (dlg.ShowDialog() == true)
            {
                return AddPlugin(dlg.FileName, channel, source, onError);
            }
            else
            {
                return false;
            }
        }

        public bool AddPlugin(string pluginPath, MixerChannel channel, ISampleProvider source, Action<Exception> onError)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub(); // TODO
                Plugin plugin = new Plugin(pluginPath, hostCmdStub);

                //플러그인에 이전 source를 추가해줘야함
                plugin.source = source;
                channel.MixerSendInput(plugin.SampleProvider);
                
                Plugins.Add(plugin);

                return true;
            }
            catch (Exception e)
            {
                onError?.Invoke(e);

                return false;
            }
        }

        public void PluginDispose(Plugin plugin)
        {
            plugin.Dispose();
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
