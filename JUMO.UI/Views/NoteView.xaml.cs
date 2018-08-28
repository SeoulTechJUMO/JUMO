using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for NoteView.xaml
    /// </summary>
    public partial class NoteView : UserControl
    {
        public NoteView()
        {
            InitializeComponent();
        }

        private void ResizeHandle_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizeComplete(DataContext);
        }

        private void ResizeHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizing(DataContext, e.HorizontalChange);
        }
    }
}
