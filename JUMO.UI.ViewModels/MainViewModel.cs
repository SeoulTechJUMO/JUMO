using System;
using System.Collections;
using System.ComponentModel;

namespace JUMO.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string FileDialogDefaultExt = ".jumo";
        private const string FileDialogFilter = "JUMO 프로젝트 (.jumo)|*.jumo|모든 파일|*.*";

        private readonly Lazy<SettingsViewModel> _settingsViewModel = new Lazy<SettingsViewModel>(() => new SettingsViewModel());

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
        /// 기존의 프로젝트 파일을 열도록 하는 명령을 가져옵니다.
        /// </summary>
        public RelayCommand OpenProjectCommand { get; }

        /// <summary>
        /// 현재 열려 있는 프로젝트를 저장하도록 하는 명령을 가져옵니다.
        /// </summary>
        public RelayCommand SaveProjectCommand { get; }

        /// <summary>
        /// 현재 열려 있는 프로젝트를 다른 이름으로 저장하도록 하는 명령을 가져옵니다.
        /// </summary>
        public RelayCommand SaveProjectAsCommand { get; }

        /// <summary>
        /// 재생을 시작하거나 멈추도록 하는 명령을 가져옵니다.
        /// </summary>
        public RelayCommand TogglePlaybackCommand { get; }

        /// <summary>
        /// 시퀀서의 재생 모드(곡/패턴)를 변경하는 명령을 가져옵니다.
        /// </summary>
        public RelayCommand TogglePlaybackModeCommand { get; }

        /// <summary>
        /// 프로그램 설정 창을 여는 명령을 가져옵니다.
        /// </summary>
        public RelayCommand OpenSettingsCommand { get; }

        public MainViewModel()
        {
            OpenProjectCommand = new RelayCommand(ExecuteOpenProject);
            SaveProjectCommand = new RelayCommand(ExecuteSaveProject);
            SaveProjectAsCommand = new RelayCommand(ExecuteSaveProjectAs);
            TogglePlaybackCommand = new RelayCommand(ExecuteTogglePlayback);
            TogglePlaybackModeCommand = new RelayCommand(ExecuteTogglePlaybackMode);
            OpenSettingsCommand = new RelayCommand(ExecuteOpenSettings);

            WorkspaceManager.Instance.PropertyChanged += WorkspaceManager_PropertyChanged;
            Song.PropertyChanged += OnSongPropertyChanged;
        }

        private void ExecuteOpenProject()
        {
            FileDialogViewModel fdvm = new FileDialogViewModel()
            {
                Title = "프로젝트 열기",
                Extension = FileDialogDefaultExt,
                Filter = FileDialogFilter
            };

            fdvm.ShowOpenCommand.Execute(null);

            if (fdvm.FileName != null)
            {
                File.ProjectReader.LoadFile(fdvm.FileName);
            }
        }

        private void ExecuteSaveProject()
        {
            if (string.IsNullOrEmpty(Song.FilePath))
            {
                ExecuteSaveProjectAs();
            }
            else
            {
                File.ProjectWriter.SaveFile(Song.FilePath);
            }
        }

        private void ExecuteSaveProjectAs()
        {
            FileDialogViewModel fdvm = new FileDialogViewModel()
            {
                Title = "프로젝트를 다른 이름으로 저장",
                Extension = FileDialogDefaultExt,
                Filter = FileDialogFilter
            };

            fdvm.ShowSaveCommand.Execute(null);

            if (fdvm.FileName != null)
            {
                File.ProjectWriter.SaveFile(fdvm.FileName);
            }
        }

        private void ExecuteTogglePlayback()
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

        private void ExecuteTogglePlaybackMode()
        {
            Sequencer.Mode =
                Sequencer.Mode == Playback.PlaybackMode.Song
                ? Playback.PlaybackMode.Pattern
                : Playback.PlaybackMode.Song;
        }

        private void ExecuteOpenSettings()
        {
            Services.WindowManagerService.Instance.ShowWindowModal(_settingsViewModel.Value);
        }

        private void WorkspaceManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => OnPropertyChanged(e.PropertyName);

        private void OnSongPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Song.Title))
            {
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }
}
