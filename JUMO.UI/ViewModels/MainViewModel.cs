using System.Collections;
using System.ComponentModel;

namespace JUMO.UI
{
    class MainViewModel : ViewModelBase
    {
        public override string DisplayName => $"{Song.Title} - JUMO";

        /// <summary>
        /// 현재 열려 있는 모든 작업 영역(탭)들을 가져옵니다.
        /// </summary>
        public IEnumerable Workspaces => WorkspaceManager.Instance.Workspaces;

        /// <summary>
        /// 현재 선택되어 화면에 표시되고 있는 작업 영역을 가져옵니다.
        /// </summary>
        public WorkspaceViewModel CurrentWorkspace
        {
            get => WorkspaceManager.Instance.CurrentWorkspace;
            set
            {
                WorkspaceManager.Instance.CurrentWorkspace = value;
                OnPropertyChanged(nameof(CurrentWorkspace));
            }
        }

        /// <summary>
        /// 현재 열려 있는 프로젝트에 대한 인스턴스를 가져옵니다.
        /// </summary>
        public Song Song { get; } = Song.Current;

        /// <summary>
        /// 곡이나 패턴을 재생할 수 있는 시퀀서 인스턴스를 가져옵니다.
        /// </summary>
        public Playback.MasterSequencer Sequencer { get; } = Playback.MasterSequencer.Instance;

        /// <summary>
        /// 플레이리스트 작업 영역을 여는 명령을 가져옵니다.
        /// </summary>
        public RelayCommand OpenPlaylistCommand { get; } =
            new RelayCommand(
                _ => WorkspaceManager.Instance.OpenWorkspace(PlaylistWorkspaceKey.Instance, () => new PlaylistViewModel())
            );

        /// <summary>
        /// 재생을 시작하거나 멈추도록 하는 명령을 가져옵니다.
        /// </summary>
        public RelayCommand TogglePlaybackCommand { get; }

        /// <summary>
        /// 시퀀서의 재생 모드(곡/패턴)를 변경하는 명령을 가져옵니다.
        /// </summary>
        public RelayCommand TogglePlaybackModeCommand { get; }

        public MainViewModel()
        {
            TogglePlaybackCommand = new RelayCommand(ExecuteTogglePlayback);
            TogglePlaybackModeCommand = new RelayCommand(ExecuteTogglePlaybackMode);

            WorkspaceManager.Instance.PropertyChanged += WorkspaceManager_PropertyChanged;
        }

        private void ExecuteTogglePlayback(object _)
        {
            if (Sequencer.IsPlaying)
            {
                Sequencer.Stop();
            }
            else
            {
                Sequencer.Continue();
            }
        }

        private void ExecuteTogglePlaybackMode(object _)
        {
            Sequencer.Mode =
                Sequencer.Mode == Playback.PlaybackMode.Song
                ? Playback.PlaybackMode.Pattern
                : Playback.PlaybackMode.Song;
        }

        private void WorkspaceManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => OnPropertyChanged(e.PropertyName);
    }
}
