using System;
using System.Windows.Controls;

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for PatternManagerView.xaml
    /// </summary>
    public partial class PatternManagerView : UserControl
    {
        public PatternManagerView()
        {
            InitializeComponent();
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            nameTextBox.Focus();
            nameTextBox.SelectAll();
        }
    }
}
