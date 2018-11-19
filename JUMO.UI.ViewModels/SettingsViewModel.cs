using System.Collections.Generic;

namespace JUMO.UI.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private SettingsGroupViewModel _currentGroup;

        #region Properties

        public override string DisplayName => "JUMO 설정";

        public List<SettingsGroupViewModel> SettingsGroups { get; } = new List<SettingsGroupViewModel>()
        {
            new ProjectSettingsViewModel(),
            new AudioSettingsViewModel()
        };

        public SettingsGroupViewModel CurrentGroup
        {
            get => _currentGroup;
            set
            {
                _currentGroup = value;
                OnPropertyChanged(nameof(CurrentGroup));
            }
        }

        #endregion

        #region Command Properties

        public RelayCommand SaveCommand { get; }
        public RelayCommand SaveAndCloseCommand { get; }

        #endregion

        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(ExecuteSave);
            SaveAndCloseCommand = new RelayCommand(ExecuteSaveAndClose);

            CurrentGroup = SettingsGroups[0];
        }

        private void ExecuteSave() => SettingsGroups.ForEach(vm => vm.SaveSettings());

        private void ExecuteSaveAndClose()
        {
            SaveCommand.Execute(null);
            CloseCommand.Execute(null);
        }
    }
}
