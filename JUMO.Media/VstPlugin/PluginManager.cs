using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using JUMO.Media.Audio;

namespace JUMO.Media.VstPlugin
{
    public sealed class PluginManager : IDisposable
    {
        #region Singleton

        private static readonly Lazy<PluginManager> _instance = new Lazy<PluginManager>(() => new PluginManager());
        private PluginManager()
        {
            _plugins = new ObservableCollection<IVstPluginContext>();
            Plugins = CollectionViewSource.GetDefaultView(_plugins);
            AudioManager.Instance.OutputDeviceChanged += AudioOutputDeviceChanged;
        }
        public static PluginManager Instance => _instance.Value;

        #endregion

        private readonly ObservableCollection<IVstPluginContext> _plugins;

        public ICollectionView Plugins { get; private set; }

        public bool AddPlugin(string pluginPath, Action<Exception> onError)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub();
                VstPluginContext ctx = VstPluginContext.Create(pluginPath, hostCmdStub);
                IVstPluginCommandStub pluginCmdStub = ctx.PluginCommandStub;

                pluginCmdStub.Open();
                pluginCmdStub.SetSampleRate(44100.0f);
                pluginCmdStub.SetBlockSize(2048);
                pluginCmdStub.MainsChanged(true);
                AudioManager.Instance.AddMixerInput(new VstSampleProvider(pluginCmdStub));

                _plugins.Add(ctx);

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
                plugin.PluginCommandStub.MainsChanged(false);
                plugin.PluginCommandStub.Close();
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
                AudioManager.Instance.AddMixerInput(new VstSampleProvider(plugin.PluginCommandStub));
            }
        }
    }
}
