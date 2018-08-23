using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUMO.UI.Layouts;

namespace JUMO.UI
{
    /// <summary>
    /// 열려 있는 VST 플러그인 편집기 창을 관리합니다.
    /// </summary>
    sealed class PluginEditorManager
    {
        #region Signleton

        private static Lazy<PluginEditorManager> _instance = new Lazy<PluginEditorManager>(() => new PluginEditorManager());

        public static PluginEditorManager Instance => _instance.Value;

        private PluginEditorManager() { }

        #endregion

        private readonly IDictionary<Vst.Plugin, PluginEditorWindow> _table = new Dictionary<Vst.Plugin, PluginEditorWindow>();

        /// <summary>
        /// 주어진 VST 플러그인의 편집기 창을 엽니다.
        /// 창이 열려있지 않은 경우에는 새로운 창 인스턴스가 생성되고,
        /// 이미 열려있는 경우에는 기존의 창을 활성화합니다.
        /// </summary>
        /// <param name="plugin">VST 플러그인</param>
        public void OpenEditor(Vst.Plugin plugin)
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
                _table.Add(plugin, newWindow);

                newWindow.Show();
            }
        }

        /// <summary>
        /// 주어진 VST 플러그인의 편집기 창을 닫습니다.
        /// 해당하는 창이 열려있지 않은 경우 아무런 동작도 수행하지 않습니다.
        /// </summary>
        /// <param name="plugin">VST 플러그인</param>
        public void CloseEditor(Vst.Plugin plugin)
        {
            _table.TryGetValue(plugin, out PluginEditorWindow window);
            window?.Close();
        }

        private void PluginWindow_Closed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PluginEditorManager::PluginWindow_Closed called");

            if (sender is PluginEditorWindow window)
            {
                _table.Remove(window.Plugin);
            }
        }
    }
}
