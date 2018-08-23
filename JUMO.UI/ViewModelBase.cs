using System;
using System.ComponentModel;

namespace JUMO.UI
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public abstract string DisplayName { get; }

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public abstract class WorkspaceViewModel : ViewModelBase
    {
        private RelayCommand _closeCommand;

        public event EventHandler CloseRequested;

        public abstract WorkspaceKey Key { get; }

        public RelayCommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(
                        _ => CloseRequested?.Invoke(this, EventArgs.Empty)
                    );
                }

                return _closeCommand;
            }
        }
    }
}
