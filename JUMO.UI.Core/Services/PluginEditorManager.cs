using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using JUMO.UI.Controls;

namespace JUMO.UI
{
    /// <summary>
    /// 열려 있는 VST 플러그인 편집기 창을 관리합니다.
    /// </summary>
    public sealed class PluginEditorManager
    {
        #region Signleton

        private static readonly Lazy<PluginEditorManager> _instance = new Lazy<PluginEditorManager>(() => new PluginEditorManager());

        public static PluginEditorManager Instance => _instance.Value;

        private PluginEditorManager()
        {
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(25), DispatcherPriority.Input, OnTimerTick, Application.Current.Dispatcher)
            {
                IsEnabled = false
            };
        }

        #endregion

        private readonly DispatcherTimer _timer;
        private readonly IDictionary<Vst.PluginBase, PluginEditorWindow> _table = new Dictionary<Vst.PluginBase, PluginEditorWindow>();

        /// <summary>
        /// 주어진 VST 플러그인의 편집기 창을 엽니다.
        /// 창이 열려있지 않은 경우에는 새로운 창 인스턴스가 생성되고,
        /// 이미 열려있는 경우에는 기존의 창을 활성화합니다.
        /// </summary>
        /// <param name="plugin">VST 플러그인</param>
        public void OpenEditor(Vst.PluginBase plugin)
        {
            if (_table.TryGetValue(plugin, out PluginEditorWindow window))
            {
                // 이미 열려있는 창을 활성화
                window.Activate();
            }
            else
            {
                // 새로운 창 인스턴스 생성 후 열기
                PluginEditorWindow newWindow = new PluginEditorWindow(plugin);
                newWindow.Closed += PluginWindow_Closed;
                plugin.Disposed += OnPluginDisposed;

                if (_table.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("PluginEditorManager::OpenEditor Starting the timer.");
                    _timer.Start();
                }

                _table.Add(plugin, newWindow);
                newWindow.Show();
            }
        }

        /// <summary>
        /// 주어진 VST 플러그인의 편집기 창을 닫습니다.
        /// 해당하는 창이 열려있지 않은 경우 아무런 동작도 수행하지 않습니다.
        /// </summary>
        /// <param name="plugin">VST 플러그인</param>
        public void CloseEditor(Vst.PluginBase plugin)
        {
            _table.TryGetValue(plugin, out PluginEditorWindow window);
            window?.Close();

            plugin.Disposed -= OnPluginDisposed;
        }

        private void PluginWindow_Closed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PluginEditorManager::PluginWindow_Closed called");

            if (sender is PluginEditorWindow window)
            {
                _table.Remove(window.Plugin);

                if (_table.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("PluginEditorManager::PluginWindow_Closed Stopping the timer.");
                    _timer.Stop();
                }
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            foreach (Vst.PluginBase plugin in _table.Keys)
            {
                plugin.PluginCommandStub.EditorIdle();
            }
        }

        private void OnPluginDisposed(object sender, EventArgs e) => CloseEditor(sender as Vst.PluginBase);
    }
}
