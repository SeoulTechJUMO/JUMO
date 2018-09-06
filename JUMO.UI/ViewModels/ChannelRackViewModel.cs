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
        private readonly Song _song = Song.Current;
        private readonly IEnumerable<Plugin> _plugins = PluginManager.Instance.Plugins;
        private ICollectionView _pluginsView;

        public override string DisplayName => $"패턴: {Pattern.Name}";

        public Pattern Pattern => _song.CurrentPattern;

        public IEnumerable<KeyValuePair<Plugin, Score>> Plugins
        {
            get
            {
                if (Pattern != null)
                {
                    foreach (Plugin p in _plugins)
                    {
                        yield return new KeyValuePair<Plugin, Score>(p, Pattern[p]);
                    }
                }
            }
        }

        public RelayCommand AddPluginCommand { get; } =
            new RelayCommand(
                _ => PluginManager.Instance.AddPlugin(null)
            );

        public RelayCommand OpenPluginEditorCommand { get; } =
            new RelayCommand(
                plugin => PluginEditorManager.Instance.OpenEditor(plugin as Plugin),
                plugin => true // TODO: VST 플러그인이 에디터 UI를 제공하는지 확인해야 함. (Flag, CanDo 등을 조사)
            );

        public RelayCommand OpenPianoRollCommand { get; } =
            new RelayCommand(
                parameter => {
                    Plugin plugin = parameter as Plugin;
                    WorkspaceKey key = new PianoRollWorkspaceKey(plugin);
                    WorkspaceManager.Instance.OpenWorkspace(key, () => new PianoRollViewModel(plugin));
                }
            );

        public ChannelRackViewModel()
        {
            _pluginsView = CollectionViewSource.GetDefaultView(_plugins);
            _pluginsView.CollectionChanged += PluginsView_CollectionChanged;
            _song.PropertyChanged += OnSongPropertyChanged;
        }

        private void PluginsView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Plugins));
        }

        private void OnSongPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Song.CurrentPattern):
                    OnPropertyChanged(nameof(Pattern));
                    OnPropertyChanged(nameof(Plugins));
                    break;
            }
        }
    }
}
