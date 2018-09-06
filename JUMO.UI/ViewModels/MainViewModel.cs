using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    class MainViewModel : ViewModelBase
    {
        public override string DisplayName => "TODO";

        public IEnumerable Workspaces => WorkspaceManager.Instance.Workspaces;
        public WorkspaceViewModel CurrentWorkspace
        {
            get => WorkspaceManager.Instance.CurrentWorkspace;
            set
            {
                WorkspaceManager.Instance.CurrentWorkspace = value;
                OnPropertyChanged(nameof(CurrentWorkspace));
            }
        }

        public RelayCommand OpenPlaylistCommand { get; } =
            new RelayCommand(
                _ => WorkspaceManager.Instance.OpenWorkspace(PlaylistWorkspaceKey.Instance, () => new PlaylistViewModel())
            );

        public MainViewModel()
        {
            WorkspaceManager.Instance.PropertyChanged += WorkspaceManager_PropertyChanged;
        }

        private void WorkspaceManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => OnPropertyChanged(e.PropertyName);
    }
}
