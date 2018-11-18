using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace JUMO.UI.ViewModels
{
    sealed class WorkspaceManager : INotifyPropertyChanged
    {
        #region Singleton

        private static Lazy<WorkspaceManager> _instance = new Lazy<WorkspaceManager>(() => new WorkspaceManager());

        public static WorkspaceManager Instance => _instance.Value;

        private WorkspaceManager() { }

        #endregion

        private readonly ObservableCollection<WorkspaceViewModel> _workspaces = new ObservableCollection<WorkspaceViewModel>();
        private WorkspaceViewModel _currentFocus;

        public event PropertyChangedEventHandler PropertyChanged;

        public IList<WorkspaceViewModel> Workspaces => _workspaces;

        public WorkspaceViewModel CurrentWorkspace
        {
            get => _currentFocus;
            set
            {
                _currentFocus = value;
                OnPropertyChanged(nameof(CurrentWorkspace));
            }
        }

        public void OpenWorkspace(WorkspaceKey key, Func<WorkspaceViewModel> workspaceProvider)
        {
            WorkspaceViewModel match = _workspaces.FirstOrDefault(ws => ws.Key.Equals(key));

            if (match != null)
            {
                CurrentWorkspace = match;
            }
            else
            {
                WorkspaceViewModel workspace = workspaceProvider();
                workspace.CloseRequested += Workspace_CloseRequested;
                _workspaces.Add(workspace);
                CurrentWorkspace = workspace;
            }
        }

        private void Workspace_CloseRequested(object sender, EventArgs e)
        {
            if (!(sender is WorkspaceViewModel workspace))
            {
                return;
            }

            workspace.CloseRequested -= Workspace_CloseRequested;
            _workspaces.Remove(workspace);
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
