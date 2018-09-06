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

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView : UserControl
    {
        public PlaylistView()
        {
            InitializeComponent();
        }

        private void MusicalCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                PlaylistViewModel vm = (PlaylistViewModel)DataContext;
                RelayCommand zoomCmd = e.Delta > 0 ? vm.ZoomInCommand : vm.ZoomOutCommand;

                if (zoomCmd.CanExecute(null))
                {
                    zoomCmd.Execute(null);
                }

                e.Handled = true;
            }
        }
    }
}
