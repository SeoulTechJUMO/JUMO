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
            if (sender.Equals(MainScrollViewer))
            {
                KeyScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
                KeyScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            }
            else
            {
                MainScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
                MainScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }
    }
}
