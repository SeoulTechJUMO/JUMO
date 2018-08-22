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
using System.Windows.Shapes;

namespace JUMO.UI.Layouts
{
    public partial class ChannelRackWindow : Window
    {
        public ChannelRackWindow()
        {
            InitializeComponent();
        }

        private void CanExecuteOpenPianoRoll(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenPianoRollExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Vst.Plugin plugin = e.Parameter as Vst.Plugin;
            Pattern pattern = (DataContext as ChannelRackViewModel).Pattern;
            PianoRollViewModel viewModel = new PianoRollViewModel(plugin, pattern);
            PianoRollWindow window = new PianoRollWindow() { DataContext = viewModel };

            window.Show();
        }
    }
}
