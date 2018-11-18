using System.Collections.ObjectModel;

namespace JUMO.UI.ViewModels
{
    public class PatternManagerViewModel : ViewModelBase
    {
        public enum Status
        {
            Normal,
            Adding,
            Renaming
        }

        private bool _isPopupVisible = false;
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

        public bool IsPopupVisible
        {
            get => _isPopupVisible;
            set
            {
                _isPopupVisible = value;
                OnPropertyChanged(nameof(IsPopupVisible));
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
            RemovePatternCommand = new RelayCommand(ExecuteRemovePattern, MoreThanOnePatterns);
            RenamePatternCommand = new RelayCommand(ExecuteRenamePattern);
            MoveUpCommand = new RelayCommand(ExecuteMoveUp, MoreThanOnePatterns);
            MoveDownCommand = new RelayCommand(ExecuteMoveDown, MoreThanOnePatterns);
            OpenAddCommand = new RelayCommand(ExecuteOpenAdd);
            OpenRenameCommand = new RelayCommand(ExecuteOpenRename);
            CloseCommand = new RelayCommand(ExecuteClose);
        }

        private bool MoreThanOnePatterns() => Song.Patterns.Count > 1;

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
            ObservableCollection<Pattern> patterns = Song.Patterns;
            int oldIdx = patterns.IndexOf(current);

            if (oldIdx <= 0)
            {
                // Wrap-around
                patterns.Move(oldIdx, patterns.Count - 1);
            }
            else
            {
                patterns.Move(oldIdx, oldIdx - 1);
            }
        }

        private void ExecuteMoveDown(object parameter)
        {
            Pattern current = parameter as Pattern;
            ObservableCollection<Pattern> patterns = Song.Patterns;
            int oldIdx = patterns.IndexOf(current);

            if (oldIdx >= patterns.Count - 1)
            {
                // Wrap-around
                patterns.Move(oldIdx, 0);
            }
            else
            {
                patterns.Move(oldIdx, oldIdx + 1);
            }
        }

        private void ExecuteOpenAdd()
        {
            CurrentStatus = Status.Adding;
            IsPopupVisible = true;
        }

        private void ExecuteOpenRename(object parameter)
        {
            Pattern current = parameter as Pattern;

            Song.CurrentPattern = current;
            CurrentName = current.Name;
            CurrentStatus = Status.Renaming;
            IsPopupVisible = true;
        }

        private void ExecuteClose()
        {
            CurrentName = "";
            CurrentStatus = Status.Normal;
            IsPopupVisible = false;
        }
    }
}
