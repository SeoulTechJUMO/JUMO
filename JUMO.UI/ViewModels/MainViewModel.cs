﻿using System.Collections;
using System.ComponentModel;

namespace JUMO.UI
{
    class MainViewModel : ViewModelBase
    {
        private const string FileDialogDefaultExt = ".jumo";
        private const string FileDialogFilter = "JUMO 프로젝트 (.jumo)|*.jumo|모든 파일|*.*";

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

        public MainViewModel()
        {
            OpenProjectCommand = new RelayCommand(ExecuteOpenProject);
            SaveProjectAsCommand = new RelayCommand(ExecuteSaveProjectAs);
            TogglePlaybackCommand = new RelayCommand(ExecuteTogglePlayback);
            TogglePlaybackModeCommand = new RelayCommand(ExecuteTogglePlaybackMode);

            WorkspaceManager.Instance.PropertyChanged += WorkspaceManager_PropertyChanged;
            Song.PropertyChanged += OnSongPropertyChanged;
        }

        #region PatternHandleCommands

        private RelayCommand _addPatternCommand;
        private RelayCommand _removePatternCommand;
        private RelayCommand _changePatternNameCommand;
        private RelayCommand _moveUpCommand;
        private RelayCommand _moveDownCommand;
        private RelayCommand _openRenameCommand;
        private RelayCommand _openAddCommand;
        private string _currentName;
        private bool _addVisible = false;
        private bool _renameVisible = false;

        public RelayCommand AddPatternCommand => _addPatternCommand ?? (_addPatternCommand = new RelayCommand(ExecuteAddPattern));
        public RelayCommand RemovePatternCommand => _removePatternCommand ?? (_removePatternCommand = new RelayCommand(ExecuteRemovePattern, _ => Song.Patterns.Count > 1));
        public RelayCommand ChangePatternNameCommand => _changePatternNameCommand ?? (_changePatternNameCommand = new RelayCommand(ExecuteChangePatternName));
        public RelayCommand MoveUpCommand => _moveUpCommand ?? (_moveUpCommand = new RelayCommand(ExecuteMoveUp));
        public RelayCommand MoveDownCommand => _moveDownCommand ?? (_moveDownCommand = new RelayCommand(ExecuteMoveDown));
        public RelayCommand OpenRenameCommand => _openRenameCommand ?? (_openRenameCommand = new RelayCommand(ExecuteOpenRenameView));
        public RelayCommand OpenAddCommand => _openAddCommand ?? (_openAddCommand = new RelayCommand(_ => AddVisible = true));

        public string CurrentName
        {
            get => _currentName;
            set
            {
                _currentName = value;
                OnPropertyChanged(nameof(CurrentName));
            }
        }

        public bool AddVisible
        {
            get => _addVisible;
            set
            {
                _addVisible = value;
                OnPropertyChanged(nameof(AddVisible));
            }
        }

        public bool RenameVisible
        {
            get => _renameVisible;
            set
            {
                _renameVisible = value;
                OnPropertyChanged(nameof(RenameVisible));
            }
        }

        private void ExecuteAddPattern()
        {
            Song.AddPattern(CurrentName);

            CurrentName = "";
        }

        private void ExecuteRemovePattern(object parameter)
        {
            Pattern current = parameter as Pattern;

            int idx = Song.Patterns.IndexOf(current);

            if (Song.Patterns.Count - 1 != idx)
            {
                Song.CurrentPattern = Song.Patterns[idx + 1];
            }
            else
            {
                Song.CurrentPattern = Song.Patterns[idx - 1];
            }

            Song.Patterns.Remove(current);
        }

        private void ExecuteOpenRenameView(object parameter)
        {
            Pattern current = parameter as Pattern;

            RenameVisible = true;
            Song.CurrentPattern = current;
            CurrentName = current.Name;
        }

        private void ExecuteChangePatternName()
        {
            if (CurrentName != "")
            {
                Song.CurrentPattern.Name = CurrentName;
            }
            CurrentName = "";
        }

        private void ExecuteMoveUp(object parameter)
        {
            Pattern current = parameter as Pattern;

            int oldidx = Song.Patterns.IndexOf(current);
            if (oldidx - 1 > 0)
            {
                Song.Patterns.Move(oldidx, oldidx - 1);
            }
            else
            {
                Song.Patterns.Move(oldidx, Song.Patterns.Count - 1);
            }
        }

        private void ExecuteMoveDown(object parameter)
        {
            Pattern current = parameter as Pattern;

            int oldidx = Song.Patterns.IndexOf(current);
            if (oldidx + 1 < Song.Patterns.Count)
            {
                Song.Patterns.Move(oldidx, oldidx + 1);
            }
            else
            {
                Song.Patterns.Move(oldidx, 0);
            }
        }

        #endregion

        private void ExecuteOpenProject(object _)
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
                new File.ProjectReader().LoadFile(fdvm.FileName);
            }
        }

        private void ExecuteSaveProjectAs(object _)
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
                new File.ProjectWriter().SaveFile(fdvm.FileName);
            }
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

        private void OnSongPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Song.Title))
            {
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }
}
