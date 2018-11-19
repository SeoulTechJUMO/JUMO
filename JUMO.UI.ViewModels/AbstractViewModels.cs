using System;

namespace JUMO.UI.ViewModels
{
    public abstract class WorkspaceViewModel : ViewModelBase
    {
        private RelayCommand _closeCommand;

        public event EventHandler CloseRequested;

        internal abstract WorkspaceKey Key { get; }

        public RelayCommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(_ => CloseRequested?.Invoke(this, EventArgs.Empty)));
    }

    public abstract class SettingsGroupViewModel : ViewModelBase
    {
        public abstract void SaveSettings();
    }
}
