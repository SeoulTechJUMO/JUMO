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

        private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                (DataContext as PianoRollViewModel).ZoomFactor += e.Delta > 0 ? 1 : -1;
                e.Handled = true;
            }
        }

        private void PianoRollCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PianoRollViewModel vm = (PianoRollViewModel)DataContext;
            Point pt = e.GetPosition((UIElement)sender);
            byte value = (byte)(127 - ((int)pt.Y / 20));
            int pos = (int)(pt.X * vm.TimeResolution / (vm.ZoomFactor << 2));
            int gridTick = (int)(vm.TimeResolution * (4.0 / vm.GridUnit));
            int snap = (int)(Math.Round((double)pos / gridTick) * gridTick);

            vm.Notes.Add(new Note(value, 64, snap, 480));
        }
    }
}
