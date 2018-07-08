using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Jacobi.Vst.Interop.Host;

namespace VstHostTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenFileDialog fileDialog = new OpenFileDialog();

        public MainWindow()
        {
            InitializeComponent();
        }

        // TODO: This is a prototype method. REPLACE WITH WPF COMMAND!
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel viewModel = DataContext as MainViewModel;

            fileDialog.Filter = "VST 플러그인 (*.dll)|*.dll";

            if (fileDialog.ShowDialog() == true)
            {
                viewModel.AddPlugin(fileDialog.FileName, null);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PluginEditorWindow editorWindow = new PluginEditorWindow();
            editorWindow.PluginCmdStub = (pluginListView.SelectedItem as VstPluginContext).PluginCommandStub;
            editorWindow.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).ChangeCurrentAudioOutputDevice();
        }
    }
}
