using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Jacobi.Vst.Core.Host;

namespace JUMO.UI.Controls
{
    /// <summary>
    /// VST 플러그인의 편집기 UI를 호스트하는 HwndHost입니다.
    /// </summary>
    public class PluginEditorHost : HwndHost
    {
        private readonly Vst.PluginBase _plugin;
        private readonly IVstPluginCommandStub _pluginCmdStub;

        /// <summary>
        /// 새로운 PluginEditorHost 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="plugin">편집기를 열 VST 플러그인</param>
        public PluginEditorHost(Vst.PluginBase plugin)
        {
            _plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            _pluginCmdStub = plugin.PluginCommandStub;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            Rectangle editorRect = new Rectangle();

            if (_pluginCmdStub.EditorGetRect(out editorRect))
            {
                Width = editorRect.Width;
                Height = editorRect.Height;

                if (_pluginCmdStub.EditorOpen(hwndParent.Handle))
                {
                    System.Diagnostics.Debug.WriteLine($"Parent hWnd = {hwndParent.Handle}");

                    List<IntPtr> childrenHWnd = new List<IntPtr>();

                    EnumChildWindows(hwndParent.Handle, (hWnd, lParam) =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Child hWnd = {hWnd}");
                        childrenHWnd.Add(hWnd);

                        return 0;
                    }, IntPtr.Zero);

                    _plugin.UpdateDisplayRequested += OnUpdateDisplayRequested;

                    return new HandleRef(this, childrenHWnd[0]);
                }

                return new HandleRef(this, IntPtr.Zero);
            }

            return new HandleRef(this, IntPtr.Zero);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            System.Diagnostics.Debug.WriteLine("PluginEditorHost::DestroyWindowCore called");
            _plugin.UpdateDisplayRequested -= OnUpdateDisplayRequested;
            _pluginCmdStub.EditorClose();
        }

        private void OnUpdateDisplayRequested(object sender, EventArgs e)
        {
            InvalidateRect(Handle, IntPtr.Zero, 0);
        }

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int EnumChildWindows(IntPtr hWndParent, EnumChildProc callback, IntPtr lParam);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int InvalidateRect(IntPtr hWnd, IntPtr lpRect, int bErase);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int EnumChildProc(IntPtr hWnd, IntPtr lParam);
    }
}
