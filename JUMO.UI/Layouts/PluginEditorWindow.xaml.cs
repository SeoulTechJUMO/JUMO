using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using JUMO.UI.Controls;

namespace JUMO.UI.Layouts
{
    // TODO: ViewModel 만들 것

    /// <summary>
    /// Interaction logic for PluginEditorWindow.xaml
    /// </summary>
    public partial class PluginEditorWindow : Window
    {
        public Vst.Plugin Plugin { get; }

        public PluginEditorWindow(Vst.Plugin plugin)
        {
            Plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            host.Child = new PluginEditorHost(Plugin);
            Title = $"플러그인 편집기: {Plugin.Name}";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                (host.Child as PluginEditorHost).Dispose();
            }
        }
    }
}
