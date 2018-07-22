using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
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
            _plugins = new ObservableCollection<Plugin>();
            Plugins = CollectionViewSource.GetDefaultView(_plugins);
            AudioManager.Instance.OutputDeviceChanged += AudioOutputDeviceChanged;
        }
        public static PluginManager Instance => _instance.Value;

        #endregion

        private readonly ObservableCollection<Plugin> _plugins;

        public ICollectionView Plugins { get; }

        public bool AddPlugin(string pluginPath, Action<Exception> onError)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub(); // TODO
                Plugin plugin = new Plugin(pluginPath, hostCmdStub);

                AudioManager.Instance.AddMixerInput(plugin.SampleProvider);
                _plugins.Add(plugin);

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
            foreach (var plugin in _plugins)
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

            foreach (var plugin in _plugins)
            {
                AudioManager.Instance.AddMixerInput(plugin.SampleProvider);
            }
        }
    }
}
