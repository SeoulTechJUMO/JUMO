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

namespace JUMO.UI.Views
{
    /// <summary>
    /// CodeMagic.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CodeMagicView : Window
    {
        public CodeMagicView(getAPI API, ObservableCollection<Progress> progress_list)
        {
            InitializeComponent();
            this.DataContext = new ChordMagicViewModel("C","Major",API,progress_list);
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            ChordMagicianModel.Properties.Settings.Default.username = "";
            ChordMagicianModel.Properties.Settings.Default.password = "";
            ChordMagicianModel.Properties.Settings.Default.Save();
            new LoginView().Show();
            this.Close();
        }
    }
}
