namespace JUMO.UI
{
    class PatternManagerViewModel : ViewModelBase
    {
        public enum Status
        {
            Normal,
            Adding,
            Renaming
        }

        private string _currentName;
        private Status _currentStatus = Status.Normal;

        #region Properties

        public override string DisplayName => "패턴 관리자";

        public Song Song { get; } = Song.Current;

        public string CurrentName
        {
            get => _currentName;
            set
            {
                _currentName = value;
                OnPropertyChanged(nameof(CurrentName));
            }
        }

        public Status CurrentStatus
        {
            get => _currentStatus;
            private set
            {
                _currentStatus = value;
                OnPropertyChanged(nameof(CurrentStatus));
            }
        }

        #endregion

        #region Commands

        public RelayCommand AddPatternCommand { get; }
        public RelayCommand RemovePatternCommand { get; }
        public RelayCommand RenamePatternCommand { get; }
        public RelayCommand MoveUpCommand { get; }
        public RelayCommand MoveDownCommand { get; }
        public RelayCommand OpenAddCommand { get; }
        public RelayCommand OpenRenameCommand { get; }
        public RelayCommand CloseCommand { get; }

        #endregion

        public PatternManagerViewModel()
        {
            AddPatternCommand = new RelayCommand(ExecuteAddPattern);
            RemovePatternCommand = new RelayCommand(ExecuteRemovePattern, _ => Song.Patterns.Count > 1);
            RenamePatternCommand = new RelayCommand(ExecuteRenamePattern);
            MoveUpCommand = new RelayCommand(ExecuteMoveUp);
            MoveDownCommand = new RelayCommand(ExecuteMoveDown);
            OpenAddCommand = new RelayCommand(ExecuteOpenAdd);
            OpenRenameCommand = new RelayCommand(ExecuteOpenRename);
            CloseCommand = new RelayCommand(ExecuteClose);
        }

        private void ExecuteAddPattern()
        {
            Song.AddPattern(CurrentName);
            ExecuteClose();
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

        private void ExecuteRenamePattern()
        {
            if (CurrentName != "")
            {
                Song.CurrentPattern.Name = CurrentName;
            }

            ExecuteClose();
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

        private void ExecuteOpenAdd()
        {
            CurrentStatus = Status.Adding;
        }

        private void ExecuteOpenRename(object parameter)
        {
            Pattern current = parameter as Pattern;

            Song.CurrentPattern = current;
            CurrentName = current.Name;
            CurrentStatus = Status.Renaming;
        }

        private void ExecuteClose()
        {
            CurrentName = "";
            CurrentStatus = Status.Normal;
        }
    }
}
