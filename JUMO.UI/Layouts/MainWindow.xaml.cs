using System.Windows;
using JUMO.UI.Controls;

namespace JUMO.UI.Layouts
{
    public partial class MainWindow : WindowBase
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Mixer_Click(object sender, RoutedEventArgs e)
        {
            new MixerWindow().Show();
        }
    }
}
