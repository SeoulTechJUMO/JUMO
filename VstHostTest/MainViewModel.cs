using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Jacobi.Vst.Interop.Host;

namespace VstHostTest
{
    class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<VstPluginContext> _plugins = new ObservableCollection<VstPluginContext>();
        private VstPluginContext _selectedPlugin = null;
        private bool _canOpenPluginEditor = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public CollectionView Plugins { get; }
        public VstPluginContext SelectedPlugin
        {
            get => _selectedPlugin;
            set
            {
                _selectedPlugin = value;
                CanOpenPluginEditor = _selectedPlugin != null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPlugin)));
            }
        }
        public bool CanOpenPluginEditor
        {
            get => _canOpenPluginEditor;
            private set
            {
                _canOpenPluginEditor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanOpenPluginEditor)));
            }
        }

        public void AddPlugin(string pluginPath)
        {
            VstPluginContext ctx = OpenPlugin(pluginPath);
            _plugins.Add(ctx);
            ctx.PluginCommandStub.MainsChanged(true);
        }

        private VstPluginContext OpenPlugin(string pluginPath)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub();
                VstPluginContext ctx = VstPluginContext.Create(pluginPath, hostCmdStub);

                ctx.Set("PluginPath", pluginPath);
                ctx.Set("HostCmdStub", hostCmdStub);

                ctx.PluginCommandStub.Open();

                return ctx;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public MainViewModel()
        {
            Plugins = new ListCollectionView(_plugins);
        }
    }

    [ValueConversion(typeof(VstPluginContext), typeof(string))]
    class PluginListItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is VstPluginContext) ? (value as VstPluginContext).PluginCommandStub.GetEffectName() : value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
