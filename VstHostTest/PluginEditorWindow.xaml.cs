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
using Jacobi.Vst.Core.Host;

namespace VstHostTest
{
    /// <summary>
    /// Interaction logic for PluginEditorWindow.xaml
    /// </summary>
    public partial class PluginEditorWindow : Window
    {
        public PluginEditorWindow()
        {
            InitializeComponent();
        }

        public IVstPluginCommandStub VstPluginCommandStub { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            host.Child = new PluginEditorHost(VstPluginCommandStub);
            Title = $"플러그인 편집기: {VstPluginCommandStub.GetEffectName()}";

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
