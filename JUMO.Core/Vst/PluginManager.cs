using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Win32;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using JUMO.Media.Audio;

namespace JUMO.Vst
{
    public sealed class PluginManager : IDisposable
    {
        #region Singleton

        private static readonly Lazy<PluginManager> _instance = new Lazy<PluginManager>(() => new PluginManager());
        private PluginManager()
        {
            AudioManager.Instance.OutputDeviceChanged += AudioOutputDeviceChanged;
        }
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

                AudioManager.Instance.AddMixerInput(plugin.SampleProvider);
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

        private void AudioOutputDeviceChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PluginManager: Audio output device has changed.");

            if (AudioManager.Instance.CurrentOutputDevice == null)
            {
                return;
            }

            foreach (var plugin in Plugins)
            {
                AudioManager.Instance.AddMixerInput(plugin.SampleProvider);
            }
        }
    }
}
