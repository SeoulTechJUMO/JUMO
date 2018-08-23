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
using Microsoft.Win32;
using JUMO.Vst;

namespace VstHostTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // TODO: Remove when you're done!!!
            new JUMO.UI.Layouts.MainWindow().Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).ChangeCurrentAudioOutputDevice();
        }
    }
}
