using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;

namespace VstHostTest
{
    sealed class PluginManager : IDisposable
    {
        #region Singleton

        private static readonly Lazy<PluginManager> _instance = new Lazy<PluginManager>(() => new PluginManager());
        private PluginManager()
        {
            _plugins = new ObservableCollection<IVstPluginContext>();
            Plugins = CollectionViewSource.GetDefaultView(_plugins);
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

                ctx.PluginCommandStub.Open();
                ctx.PluginCommandStub.SetSampleRate(44100.0f);
                ctx.PluginCommandStub.SetBlockSize(2048);
                ctx.PluginCommandStub.MainsChanged(true);

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
    }
}
