using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
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
                NullHostCommandStub hostCmdStub = new NullHostCommandStub();
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

    class NullHostCommandStub : IVstHostCommandStub
    {
        public IVstPluginContext PluginContext { get; set; }

        public bool BeginEdit(int index)
        {
            throw new NotImplementedException();
        }

        public VstCanDoResult CanDo(string cando)
        {
            throw new NotImplementedException();
        }

        public bool CloseFileSelector(VstFileSelect fileSelect)
        {
            throw new NotImplementedException();
        }

        public bool EndEdit(int index)
        {
            throw new NotImplementedException();
        }

        public VstAutomationStates GetAutomationState()
        {
            throw new NotImplementedException();
        }

        public int GetBlockSize()
        {
            throw new NotImplementedException();
        }

        public int GetCurrentPluginID()
        {
            return PluginContext.PluginInfo.PluginID;
        }

        public string GetDirectory()
        {
            throw new NotImplementedException();
        }

        public int GetInputLatency()
        {
            throw new NotImplementedException();
        }

        public VstHostLanguage GetLanguage()
        {
            throw new NotImplementedException();
        }

        public int GetOutputLatency()
        {
            throw new NotImplementedException();
        }

        public VstProcessLevels GetProcessLevel()
        {
            throw new NotImplementedException();
        }

        public string GetProductString()
        {
            throw new NotImplementedException();
        }

        public float GetSampleRate()
        {
            throw new NotImplementedException();
        }

        public VstTimeInfo GetTimeInfo(VstTimeInfoFlags filterFlags)
        {
            throw new NotImplementedException();
        }

        public string GetVendorString()
        {
            throw new NotImplementedException();
        }

        public int GetVendorVersion()
        {
            throw new NotImplementedException();
        }

        public int GetVersion()
        {
            return 1000;
        }

        public bool IoChanged()
        {
            throw new NotImplementedException();
        }

        public bool OpenFileSelector(VstFileSelect fileSelect)
        {
            throw new NotImplementedException();
        }

        public bool ProcessEvents(VstEvent[] events)
        {
            throw new NotImplementedException();
        }

        public void ProcessIdle()
        {
        }

        public void SetParameterAutomated(int index, float value)
        {
        }

        public bool SizeWindow(int width, int height)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDisplay()
        {
            return false;
        }
    }
}
