using System;
using System.ComponentModel;

namespace JUMO.UI
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private RelayCommand _closeCommand;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CloseRequested;

        public abstract string DisplayName { get; }

        public RelayCommand CloseCommand
            => _closeCommand ?? (_closeCommand = new RelayCommand(ExecuteClose, CanExecuteClose));

        protected virtual void ExecuteClose() => CloseRequested?.Invoke(this, EventArgs.Empty);

        protected virtual bool CanExecuteClose() => true;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
