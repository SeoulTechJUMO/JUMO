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

        public IVstPluginCommandStub PluginCmdStub { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            host.Child = new PluginEditorHost(PluginCmdStub);
            Title = $"플러그인 편집기: {PluginCmdStub.GetEffectName()}";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                (host.Child as PluginEditorHost).Dispose();
            }
        }

        private void MiddleCMouseDown(object sender, MouseButtonEventArgs e)
        {
            PluginCmdStub.StartProcess();
            PluginCmdStub.ProcessEvents(new VstEvent[] {
                new VstMidiEvent(0, 0, 0, new byte[] { 0x90, 0x3c, 0x40, 0x00 }, 0, 127)
            });
            PluginCmdStub.StopProcess();
        }

        private void MiddleCMouseUp(object sender, MouseButtonEventArgs e)
        {
            PluginCmdStub.StartProcess();
            PluginCmdStub.ProcessEvents(new VstEvent[] {
                new VstMidiEvent(0, 0, 0, new byte[] { 0x80, 0x3c, 0x40, 0x00 }, 0, 127)
            });
            PluginCmdStub.StopProcess();
        }

        private void CMaj7MouseDown(object sender, MouseButtonEventArgs e)
        {
            PluginCmdStub.StartProcess();
            PluginCmdStub.ProcessEvents(new VstEvent[] {
                new VstMidiEvent(0, 0, 0, new byte[] { 0x90, 0x3c, 0x40, 0x00 }, 0, 127),
                new VstMidiEvent(0, 0, 0, new byte[] { 0x90, 0x40, 0x40, 0x00 }, 0, 127),
                new VstMidiEvent(0, 0, 0, new byte[] { 0x90, 0x43, 0x40, 0x00 }, 0, 127),
                new VstMidiEvent(0, 0, 0, new byte[] { 0x90, 0x47, 0x40, 0x00 }, 0, 127)
            });
            PluginCmdStub.StopProcess();
        }

        private void CMaj7MouseUp(object sender, MouseButtonEventArgs e)
        {
            PluginCmdStub.StartProcess();
            PluginCmdStub.ProcessEvents(new VstEvent[] {
                new VstMidiEvent(0, 0, 0, new byte[] { 0x80, 0x3c, 0x40, 0x00 }, 0, 127),
                new VstMidiEvent(0, 0, 0, new byte[] { 0x80, 0x40, 0x40, 0x00 }, 0, 127),
                new VstMidiEvent(0, 0, 0, new byte[] { 0x80, 0x43, 0x40, 0x00 }, 0, 127),
                new VstMidiEvent(0, 0, 0, new byte[] { 0x80, 0x47, 0x40, 0x00 }, 0, 127)
            });
            PluginCmdStub.StopProcess();
        }
    }
}
