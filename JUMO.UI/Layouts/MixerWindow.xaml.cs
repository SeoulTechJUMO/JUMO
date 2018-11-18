using System.Windows;
using JUMO.UI.ViewModels;

namespace JUMO.UI.Layouts
{
    public partial class MixerWindow : Window
    {
        public MixerWindow()
        {
            DataContext = new MixerViewModel();
            InitializeComponent();
        }
    }
}
