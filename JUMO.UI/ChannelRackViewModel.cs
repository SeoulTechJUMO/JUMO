using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using JUMO.Vst;

namespace JUMO.UI
{
    public class ChannelRackViewModel : ViewModelBase
    {
        // TODO: PluginsSource 속성을 만들어서 의존성 분리?
        private IEnumerable<Plugin> _plugins = PluginManager.Instance.Plugins;
        private ICollectionView _pluginsView;
        private Pattern _pattern;

        private RelayCommand _addPluginCommand;
        private RelayCommand _openPluginEditorCommand;

        public override string DisplayName => $"패턴: {_pattern.Name}";

        public Pattern Pattern
        {
            get => _pattern;
            set
            {
                _pattern = value;
                OnPropertyChanged(nameof(Pattern));
                OnPropertyChanged(nameof(Plugins));
            }
        }

        public IEnumerable<KeyValuePair<Plugin, IEnumerable<Note>>> Plugins
        {
            get
            {
                if (_pattern != null)
                {
                    foreach (Plugin p in _plugins)
                    {
                        yield return new KeyValuePair<Plugin, IEnumerable<Note>>(p, _pattern[p]);
                    }
                }
            }
        }

        public RelayCommand AddPluginCommand
        {
            get
            {
                if (_addPluginCommand == null)
                {
                    _addPluginCommand = new RelayCommand(_ => PluginManager.Instance.AddPlugin(null));
                }

                return _addPluginCommand;
            }
        }

        public RelayCommand OpenPluginEditorCommand
        {
            get
            {
                if (_openPluginEditorCommand == null)
                {
                    _openPluginEditorCommand = new RelayCommand(
                        plugin => PluginEditorManager.Instance.OpenEditor(plugin as Plugin),
                        plugin => true // TODO: VST 플러그인이 에디터 UI를 제공하는지 확인해야 함. (Flag, CanDo 등을 조사)
                    );
                }

                return _openPluginEditorCommand;
            }
        }

        public ChannelRackViewModel()
        {
            Pattern = new Pattern("Test Pattern");
            _pluginsView = CollectionViewSource.GetDefaultView(_plugins);
            _pluginsView.CollectionChanged += PluginsView_CollectionChanged;
        }

        private void PluginsView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Plugins));
        }
    }
}
