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
        public override string DisplayName => $"{Song.Title} - JUMO";

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

        public Song Song { get; } = Song.Current;

        public RelayCommand OpenPlaylistCommand { get; } =
            new RelayCommand(
                _ => WorkspaceManager.Instance.OpenWorkspace(PlaylistWorkspaceKey.Instance, () => new PlaylistViewModel())
            );

        public RelayCommand TogglePlaybackCommand { get; }

        public MainViewModel()
        {
            TogglePlaybackCommand = new RelayCommand(ExecuteTogglePlayback);

            WorkspaceManager.Instance.PropertyChanged += WorkspaceManager_PropertyChanged;
        }

        private void ExecuteTogglePlayback(object _)
        {
            if (Song.Sequencer.IsPlaying)
            {
                Song.Sequencer.Stop();
            }
            else
            {
                Song.Sequencer.Continue();
            }
        }

        private void WorkspaceManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => OnPropertyChanged(e.PropertyName);
    }
}
