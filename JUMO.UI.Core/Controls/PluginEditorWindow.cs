using System;
using System.ComponentModel;
using System.Windows;

namespace JUMO.UI.Controls
{
    public sealed class PluginEditorWindow : Window
    {
        private readonly PluginEditorHost _host;

        public Vst.PluginBase Plugin { get; }

        public PluginEditorWindow(Vst.PluginBase plugin)
        {
            Plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));

            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;
            Content = _host = new PluginEditorHost(plugin);
            Title = $"플러그인 편집기: {plugin.Name}";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                _host.Dispose();
            }

            base.OnClosing(e);
        }
    }
}
