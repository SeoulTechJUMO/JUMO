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
using JUMO.UI;

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
            InitializeComponent();
            _vm = vm;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((ChordMagicViewModel)DataContext).ChordChanged += OnChordChanged;
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            ChordMagicianModel.Properties.Settings.Default.Token = "";
            ChordMagicianModel.Properties.Settings.Default.Save();
            new LoginView(_vm).Show();
            Close();
        }

        private void InsertButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SmartMelodyClick(object sender, RoutedEventArgs e)
        {
            SmartMelodyViewModel svm = new SmartMelodyViewModel((ChordMagicViewModel)DataContext);
            SmartMelodyView sv = new SmartMelodyView { DataContext = svm };

            sv.ShowDialog();

            if (svm.WillInsert)
            {
                Close();
            }
        }

        private void OnChordChanged(object sender, ChordMagicViewModel.ChordChangedEventArgs e)
        {
            if (e.ResultCode == ChordMagicViewModel.ChangeChordResult.EmptyResult)
            {
                MessageBox.Show("다음으로 적합한 코드진행이 없습니다.");
            }
            else if (e.ResultCode == ChordMagicViewModel.ChangeChordResult.MissingInsert)
            {
                MessageBox.Show("코드를 선택해 주세요.");
            }
            else if (e.ResultCode == ChordMagicViewModel.ChangeChordResult.MissingRemove)
            {
                MessageBox.Show("삭제할 코드를 선택해 주세요.");
            }
        }
    }
}
