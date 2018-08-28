using JUMO.UI.Controls;
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

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for NoteVelocityView.xaml
    /// </summary>
    public partial class NoteVelocityView : UserControl
    {
        public NoteVelocityView()
        {
            InitializeComponent();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            (VisualParent as IMusicalViewCallback)?.MusicalViewResizing(DataContext, e.VerticalChange);
        }
    }
}
