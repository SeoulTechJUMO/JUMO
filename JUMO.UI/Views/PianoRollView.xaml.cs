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
using JUMO.UI.Controls;

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for PianoRollView.xaml
    /// </summary>
    public partial class PianoRollView : UserControl
    {
        public PianoRollView()
        {
            InitializeComponent();
        }

        private void MusicalCanvas_ZoomChanged(object sender, MusicalCanvasZoomEventArgs e)
        {
            ((PianoRollViewModel)DataContext).ZoomFactor += e.Delta;
        }
    }
}
