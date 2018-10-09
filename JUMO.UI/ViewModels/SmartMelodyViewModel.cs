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
using System.Windows.Threading;
using System.Collections.Specialized;

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

        //생성된 멜로디 딕셔너리
        private Dictionary<string, List<Note>> _GeneratedMelody = new Dictionary<string, List<Note>>();
        public Dictionary<string, List<Note>> GeneratedMelody
        {
            get => _GeneratedMelody;
            set
            {
                _GeneratedMelody = value;
                OnPropertyChanged(nameof(GeneratedMelody));
            }
        }

        //선택된 멜로디 딕셔너리
        private string _CurrentMelody;
        public string CurrentMelody
        {
            get => _CurrentMelody;
            set
            {
                _CurrentMelody = value;
                OnPropertyChanged(nameof(CurrentMelody));
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
                CreateMelody.RunMagenta(Chord, 5);
                MakeScore(CreateMelody.GetMelodyPath());
                ProgressVisible = Visibility.Hidden;
            });
        }

        public void MakeScore(string[] files)
        {
            GeneratedMelody.Clear();

            //삽입할 노트 리스트
            List<Note> Notes = new List<Note>();
            int Count = 0;

            //사용할 도구 객체
            MakeNote nm = new MakeNote();

            foreach (string s in files)
            {
                Count++;
                string MelodyName = "";
                MelodyName += "멜로디 " + Count;
                Notes = InsertNote(s);

                GeneratedMelody.Add(MelodyName,Notes);
            }
            CurrentMelody = GeneratedMelody.Keys.ElementAt(0);
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
