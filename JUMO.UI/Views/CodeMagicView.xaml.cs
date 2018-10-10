using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ChordMagicianModel;
using JUMO.UI;
using JUMO.UI.ViewModels;

namespace JUMO.UI.Views
{
    /// <summary>
    /// CodeMagic.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CodeMagicView : Window
    {
        PianoRollViewModel _vm;

        public CodeMagicView(PianoRollViewModel vm)
        {
            _vm = vm;
            InitializeComponent();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            // ChordMagicianModel.Properties.Settings.Default.username = "";
            // ChordMagicianModel.Properties.Settings.Default.password = "";
            ChordMagicianModel.Properties.Settings.Default.Save();
            new LoginView(_vm).Show();
            this.Close();
        }

        private void InsertButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SmartMelodyClick(object sender, RoutedEventArgs e)
        {
            SmartMelodyView sv = new SmartMelodyView();
            SmartMelodyViewModel svm = new SmartMelodyViewModel((ChordMagicViewModel)this.DataContext);
            sv.DataContext = svm;
            sv.ShowDialog();

            if (svm.InsertFlag == true)
            {
                this.Close();
            }
        }
    }
}
