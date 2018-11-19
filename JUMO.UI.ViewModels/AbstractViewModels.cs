namespace JUMO.UI.ViewModels
{
    public abstract class WorkspaceViewModel : ViewModelBase
    {
        internal abstract WorkspaceKey Key { get; }
    }

    public abstract class SettingsGroupViewModel : ViewModelBase
    {
        public abstract void SaveSettings();
    }
}
