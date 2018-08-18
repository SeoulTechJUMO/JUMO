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
    class PrototypeChannelRackViewModel : INotifyPropertyChanged
    {
        // TODO: PluginsSource 속성을 만들어서 의존성 분리?
        private IEnumerable<Plugin> _plugins = PluginManager.Instance.Plugins;
        private ICollectionView _pluginsView;
        private Pattern _pattern;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public PrototypeChannelRackViewModel()
        {
            Pattern = new Pattern("Test Pattern");
            _pluginsView = CollectionViewSource.GetDefaultView(_plugins);
            _pluginsView.CollectionChanged += PluginsView_CollectionChanged;
        }

        private void PluginsView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Plugins));
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
