using System;
using System.Windows.Input;

namespace JUMO.UI
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> execute) : this(execute, (Predicate<object>)null) { }

        public RelayCommand(Action execute) : this(_ => execute(), (Predicate<object>)null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> execute, Func<bool> canExecute) : this(execute, _ => canExecute()) { }

        public RelayCommand(Action execute, Predicate<object> canExecute) : this(_ => execute(), canExecute) { }

        public RelayCommand(Action execute, Func<bool> canExecute) : this(_ => execute(), _ => canExecute()) { }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);

        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}
