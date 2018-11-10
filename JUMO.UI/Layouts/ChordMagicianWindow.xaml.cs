using System.Windows;
using JUMO.UI.Controls;
using ChordMagicianModel;

namespace JUMO.UI.Layouts
{
    /// <summary>
    /// CodeMagic.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChordMagicianWindow : WindowBase
    {
        private readonly GetAPI _api = new GetAPI();
        private readonly ChordMagicianViewModel _viewModel;

        public ChordMagicianWindow(PianoRollViewModel pianoRollVM)
        {
            InitializeComponent();

            DataContext = _viewModel = new ChordMagicianViewModel(_api, pianoRollVM);
            _viewModel.ChordChanged += OnChordChanged;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (new HooktheoryLoginDialog(_api).ShowDialog() ?? false)
            {
                await _viewModel.ResetChords();
            }
            else
            {
                Close();
            }
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InsertButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SmartMelodyClick(object sender, RoutedEventArgs e)
        {
            if (((ChordMagicianViewModel)DataContext).CurrentProgress.Count != 0)
            {
                SmartMelodyViewModel svm = new SmartMelodyViewModel((ChordMagicianViewModel)DataContext);
                SmartMelodyWindow sv = new SmartMelodyWindow { DataContext = svm };

                sv.ShowDialog();

                if (svm.WillInsert)
                {
                    Close();
                }
            }
            else
            {
                MessageBox.Show("코드 진행을 하나 이상 추가해주세요.");
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
