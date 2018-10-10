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
        private ObservableCollection<KeyValuePair<string, List<Note>>> _GeneratedMelody = new ObservableCollection<KeyValuePair<string, List<Note>>>();
        public ObservableCollection<KeyValuePair<string, List<Note>>> GeneratedMelody
        {
            get => _GeneratedMelody;
            set
            {
                _GeneratedMelody = value;
                OnPropertyChanged(nameof(GeneratedMelody));
            }
        }

        //선택된 멜로디
        private string _CurrentMelody;
        public string CurrentMelody
        {
            get => _CurrentMelody;
            set
            {
                if (_CurrentMelody != null)
                {
                    ChangeScore(_CurrentMelody, true);
                }
                _CurrentMelody = value;
                ChangeScore(_CurrentMelody);
                OnPropertyChanged(nameof(CurrentMelody));
            }
        }

        //삽입여부
        public bool InsertFlag = false;

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

            Dispatcher dispatcher = Application.Current.Dispatcher;

            foreach (Progress progress in ViewModel.CurrentProgress)
            {
                Chord += progress.Chord;
                Chord += " ";
            }

            Task.Run(() => {
                CreateMelody.RunMagenta(Chord, 5);
                dispatcher.BeginInvoke((Action)(() =>
                {
                    MakeScore(CreateMelody.GetMelodyPath());
                    
                }));
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

                GeneratedMelody.Add(new KeyValuePair<string, List<Note>> (MelodyName,Notes));
            }
            CurrentMelody = GeneratedMelody[0].Key;
        }

        public List<Note> InsertNote(string FilePath)
        {
            List<Note> Notes = new List<Note>();
            Notes = new MakeNote().MakeScore(FilePath);

            return Notes;
        }

        private RelayCommand _Cancel;
        public RelayCommand Cancel
        {
            get
            {
                if (_Cancel == null)
                {
                    _Cancel = new RelayCommand(_ => ChangeScore(CurrentMelody, true));
                }
                return _Cancel;
            }
        }

        private RelayCommand _Insert;
        public RelayCommand Insert
        {
            get
            {
                if (_Insert == null)
                {
                    _Insert = new RelayCommand(_ => InsertFlag = true);
                }
                return _Insert;
            }
        }

        public void ChangeScore(string Current, bool Remove=false)
        {
            List<Note> Notes = new List<Note>();
            foreach (KeyValuePair<string, List<Note>> i in GeneratedMelody)
            {
                if (i.Key == Current)
                {
                    Notes = i.Value;
                    break;
                }
            }

            if (Remove == false)
            {
                foreach (Note i in Notes)
                {
                    ViewModel.ViewModel.AddNote(i);
                }
            }
            else
            {
                foreach (Note i in Notes)
                {
                    ViewModel.ViewModel.RemoveNote(i);
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
