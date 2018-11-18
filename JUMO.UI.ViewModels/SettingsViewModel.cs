using System.Collections.Generic;

namespace JUMO.UI.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private SettingsGroupViewModel _currentGroup;

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

        public SettingsViewModel() => CurrentGroup = SettingsGroups[0];

        public void SaveSettings() => SettingsGroups.ForEach(vm => vm.SaveSettings());
    }
}
