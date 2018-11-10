using System;
using System.Windows;
using System.Windows.Input;

namespace JUMO.UI.Controls
{
    public class WindowBase : Window
    {
        public WindowBase() : base()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindowExecuted, CanExecuteMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindowExecuted, CanExecuteMaximizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindowExecuted, CanExecuteRestoreWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindowExecuted, CanExecuteCloseWindow));
        }

        protected virtual void OnMinimizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        protected virtual void OnMaximizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        protected virtual void OnRestoreWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        protected virtual void OnCloseWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        protected virtual void CanExecuteMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = WindowState != WindowState.Minimized;
        }

        protected virtual void CanExecuteMaximizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode > ResizeMode.CanMinimize && WindowState != WindowState.Maximized;
        }

        protected virtual void CanExecuteRestoreWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode > ResizeMode.CanMinimize && WindowState != WindowState.Normal;
        }

        protected virtual void CanExecuteCloseWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            base.OnStateChanged(e);
        }
    }
}
