using JUMO.UI.Controls;
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
    /// <summary>
    /// Interaction logic for PianoRollWindow.xaml
    /// </summary>
    public partial class PianoRollWindow : Window
    {
        public PianoRollWindow()
        {
            InitializeComponent();
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            /*if (sender.Equals(MainScrollViewer))
            {
                KeyScrollViewer.ScrollToHorizontalOffset(0.0);
                KeyScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            }
            else
            {
                MainScrollViewer?.ScrollToVerticalOffset(e.VerticalOffset);
            }*/
        }

        private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                (DataContext as IPianoRollViewModel).ZoomFactor += e.Delta > 0 ? 1 : -1;
                e.Handled = true;
            }
        }
    }
}
