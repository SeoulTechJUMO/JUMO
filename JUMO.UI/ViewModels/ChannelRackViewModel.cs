using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using JUMO.Vst;

namespace JUMO.UI
{
    public class ChannelRackViewModel : ViewModelBase
    {
        private readonly Song _song = Song.Current;
        private readonly ObservableCollection<Plugin> _plugins = PluginManager.Instance.Plugins;
        private RelayCommand _addPluginCommand;
        private RelayCommand _replacePluginCommand;

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

        public RelayCommand AddPluginCommand => _addPluginCommand ?? (_addPluginCommand = new RelayCommand(_ => AddPlugin()));
        public RelayCommand ReplacePluginCommand => _replacePluginCommand ?? (_replacePluginCommand = new RelayCommand(plugin => AddPlugin(true, plugin as Plugin)));

        private void AddPlugin(bool replace=false, Plugin oldPlugin = null)
        {
            FileDialogViewModel fdvm = new FileDialogViewModel()
            {
                Title = "플러그인 열기",
                Extension = ".dll",
                Filter = "VST 플러그인|*.dll|모든 파일|*.*"
            };

            fdvm.ShowOpenCommand.Execute(null);

            if (fdvm.FileName != null)
            {
                if(replace)
                {
                    PluginManager.Instance.AddPlugin(fdvm.FileName, null, true, oldPlugin);
                }
                else
                {
                    PluginManager.Instance.AddPlugin(fdvm.FileName, null);
                }
            }
        }

        public RelayCommand OpenPluginEditorCommand { get; } =
            new RelayCommand(
                plugin => PluginEditorManager.Instance.OpenEditor(plugin as PluginBase),
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

        public RelayCommand RemovePluginCommand { get; } =
            new RelayCommand(
                plugin => PluginManager.Instance.RemovePlugin(plugin as Plugin)
            );

        public ChannelRackViewModel()
        {
            _plugins.CollectionChanged += OnPluginsCollectionChanged;
            _song.PropertyChanged += OnSongPropertyChanged;
        }

        private void OnPluginsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
