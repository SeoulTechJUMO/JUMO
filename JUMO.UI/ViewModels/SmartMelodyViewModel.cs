using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using ChordMagicianModel;
using System.Windows;

namespace JUMO.UI.ViewModels
{
    public class SmartMelodyViewModel : INotifyPropertyChanged
    {
        public SmartMelodyViewModel(ChordMagicViewModel vm)
        {
            _ViewModel = vm;
            _ProgressVisible = Visibility.Hidden;
        }

        //코드진행 뷰모델
        private ChordMagicViewModel _ViewModel;
        public ChordMagicViewModel ViewModel
        {
            get => _ViewModel;
            set
            {
                _ViewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }

        //프로그래스 이미지 표시유무
        private Visibility _ProgressVisible;
        public Visibility ProgressVisible
        {
            get => _ProgressVisible;
            set
            {
                _ProgressVisible = value;
                OnPropertyChanged(nameof(ProgressVisible));
            }
        }

        //생성된 멜로디 표시 스코어
        private List<Score> _GeneratedScore = new List<Score>();
        public List<Score> GeneratedScore
        {
            get => _GeneratedScore;
            set
            {
                _GeneratedScore = value;
                OnPropertyChanged(nameof(GeneratedScore));
            }
        }

        //선택중인 멜로디 스코어
        private Score _CurrentScore;
        public Score CurrentScore
        {
            get => _CurrentScore;
            set
            {
                _CurrentScore = value;
                OnPropertyChanged(nameof(CurrentScore));
            }
        }


        //사용 커맨드
        private RelayCommand _GetMelody;
        public RelayCommand GetMelody
        {
            get
            {
                if (_GetMelody == null)
                {
                    _GetMelody = new RelayCommand(_ => MakeMelody());
                }
                return _GetMelody;
            }
        }

        public void MakeMelody()
        {
            ProgressVisible = Visibility.Visible;
            string Chord = "";

            foreach (Progress progress in ViewModel.CurrentProgress)
            {
                Chord += progress.Chord;
                Chord += " ";
            }

            Task.Run(() =>
            {
                CreateMelody cm = new CreateMelody();
                cm.RunMagenta(Chord, 5);
                MakeScore(cm.GetMelodyPath());
                ProgressVisible = Visibility.Hidden;
            });
        }

        public void MakeScore(string[] files)
        {
            GeneratedScore.Clear();

            //생성할 스코어에 삽입할 임시 노트 리스트
            List<Note> Notes = new List<Note>();

            //사용할 도구 객체
            MakeNote nm = new MakeNote();

            foreach (string s in files)
            {
                //TODO: 파일별로 가상 스코어를 생성해 줘야함
                //InsertNote를 파일 수 만큼 돌려준다
            }
        }

        public List<Note> InsertNote(string FilePath)
        {
            List<Note> Notes = new List<Note>();
            Notes = new MakeNote().MakeScore(FilePath);

            return Notes;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
