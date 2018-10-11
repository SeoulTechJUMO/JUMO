using System.Windows;

namespace JUMO.UI.Layouts
{
    /// <summary>
    /// CodeMagic.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChordMagicianWindow : Window
    {
        PianoRollViewModel _vm;

        public ChordMagicianWindow(PianoRollViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((ChordMagicianViewModel)DataContext).ChordChanged += OnChordChanged;
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            ChordMagicianModel.Properties.Settings.Default.Token = "";
            ChordMagicianModel.Properties.Settings.Default.Save();
            new HooktheoryLoginDialog(_vm).Show();
            Close();
        }

        private void InsertButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SmartMelodyClick(object sender, RoutedEventArgs e)
        {
            SmartMelodyViewModel svm = new SmartMelodyViewModel((ChordMagicianViewModel)DataContext);
            SmartMelodyWindow sv = new SmartMelodyWindow { DataContext = svm };

            sv.ShowDialog();

            if (svm.WillInsert)
            {
                Close();
            }
        }

        private void OnChordChanged(object sender, ChordMagicianViewModel.ChordChangedEventArgs e)
        {
            if (e.ResultCode == ChordMagicianViewModel.ChangeChordResult.EmptyResult)
            {
                MessageBox.Show("다음으로 적합한 코드진행이 없습니다.");
            }
            else if (e.ResultCode == ChordMagicianViewModel.ChangeChordResult.MissingInsert)
            {
                MessageBox.Show("코드를 선택해 주세요.");
            }
            else if (e.ResultCode == ChordMagicianViewModel.ChangeChordResult.MissingRemove)
            {
                MessageBox.Show("삭제할 코드를 선택해 주세요.");
            }
        }
    }
}
