using System;
using System.Collections.Generic;
using System.Windows;

namespace JUMO.UI.Services
{
    public sealed class WindowManagerService
    {
        #region Singleton

        private static readonly Lazy<WindowManagerService> _instance = new Lazy<WindowManagerService>(() => new WindowManagerService());

        public static WindowManagerService Instance => _instance.Value;

        #endregion

        private readonly Dictionary<ViewModelBase, Window> _table = new Dictionary<ViewModelBase, Window>();

        private WindowManagerService() { }

        public void ShowWindow(ViewModelBase viewModel)
        {
            if (_table.TryGetValue(viewModel, out Window window))
            {
                window.Activate();
            }
            else
            {
                RegisterWindow(viewModel).Show();
            }
        }

        public bool? ShowWindowModal(ViewModelBase viewModel)
        {
            if (_table.TryGetValue(viewModel, out Window window))
            {
                return window.Activate();
            }
            else
            {
                return RegisterWindow(viewModel).ShowDialog();
            }
        }

        private Window RegisterWindow(ViewModelBase viewModel)
        {
            Window newWindow = new Window()
            {
                Content = viewModel ?? throw new ArgumentNullException(nameof(viewModel))
            };

            viewModel.CloseRequested += OnViewModelCloseRequested;
            newWindow.Closed += OnWindowClosed;
            // TODO: Do additional preparations here.

            _table.Add(viewModel, newWindow);

            return newWindow;
        }

        private void OnViewModelCloseRequested(object sender, EventArgs e)
        {
            if (_table.TryGetValue((ViewModelBase)sender, out Window window))
            {
                window.Close();
            }
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            ViewModelBase vm = (sender as Window)?.Content as ViewModelBase;

            if (_table.TryGetValue(vm, out Window window) && ReferenceEquals(window, sender))
            {
                _table.Remove(vm);
            }
        }
    }
}
