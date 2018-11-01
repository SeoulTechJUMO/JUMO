using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ChordMagicianModel;

namespace JUMO.UI
{
    public class SmartMelodyViewModel : ViewModelBase
    {
        // TODO
        private Visibility _progressVisible;
        private string _currentMelody;
        private byte _melodyCount;
        private byte _chordCount;

        private RelayCommand _getMelodyCommand;
        private RelayCommand _cancelCommand;
        private RelayCommand _insertCommand;
        private RelayCommand _MelodyPlayCommand;

        #region Properties

        public override string DisplayName => "스마트 멜로디 생성";

        //코드진행 뷰모델
        public ChordMagicianViewModel ViewModel { get; }

        //멜로디 재생용 마스터 시퀀서
        public Playback.MasterSequencer Sequencer { get; } = Playback.MasterSequencer.Instance;

        //프로그래스 이미지 표시유무
        public Visibility ProgressVisible
        {
            get => _progressVisible;
            set
            {
                _progressVisible = value;
                OnPropertyChanged(nameof(ProgressVisible));
            }
        }

        //생성된 멜로디 딕셔너리
        public ObservableCollection<KeyValuePair<string, List<Note>>> GeneratedMelody { get; } = new ObservableCollection<KeyValuePair<string, List<Note>>>();

        //선택된 멜로디
        public string CurrentMelody
        {
            get => _currentMelody;
            set
            {
                if (_currentMelody != null)
                {
                    ChangeScore(_currentMelody, true);
                }

                _currentMelody = value;

                ChangeScore(_currentMelody);
                OnPropertyChanged(nameof(CurrentMelody));
            }
        }

        //멜로디 파일 생성 개수
        public byte MelodyCount
        {
            get => _melodyCount;
            set
            {
                _melodyCount = value;
                OnPropertyChanged(nameof(MelodyCount));
            }
        }

        //코드진행 반복 횟수
        public byte ChordCount
        {
            get => _chordCount;
            set
            {
                _chordCount = value;
                OnPropertyChanged(nameof(ChordCount));
            }
        }

        //삽입여부
        public bool WillInsert { get; set; } = false;

        #endregion

        #region Command Properties

        public RelayCommand GetMelodyCommand
            => _getMelodyCommand ?? (_getMelodyCommand = new RelayCommand(_ => MakeMelody()));

        public RelayCommand CancelCommand
            => _cancelCommand ?? (_cancelCommand = new RelayCommand(_ => ChangeScore(CurrentMelody, true)));

        public RelayCommand InsertCommand
            => _insertCommand ?? (_insertCommand = new RelayCommand(_ => WillInsert = true));

        public RelayCommand MelodyPlayCommand
            => _MelodyPlayCommand ?? (_MelodyPlayCommand = new RelayCommand(_ => MelodyPlay(), _ => _currentMelody != null));

        #endregion

        public SmartMelodyViewModel(ChordMagicianViewModel vm)
        {
            ViewModel = vm;
            _progressVisible = Visibility.Hidden;
            _melodyCount = 5;
            _chordCount = 1;
            Sequencer.Mode = Playback.PlaybackMode.Pattern;
        }

        public void MakeMelody()
        {
            ProgressVisible = Visibility.Visible;
            string chord = "";

            Dispatcher dispatcher = Application.Current.Dispatcher;

            for (int i = 0; i < ChordCount; i++)
            {
                foreach (Progress progress in ViewModel.CurrentProgress)
                {
                    chord += progress.Chord;
                    chord += " ";
                }
            }

            Task.Run(() => {
                CreateMelody.RunMagenta(chord, MelodyCount);
                dispatcher.BeginInvoke((Action)(() =>
                {
                    MakeScore(CreateMelody.MelodyPath);
                }));
                ProgressVisible = Visibility.Hidden;
            });
        }

        private void MakeScore(string[] files)
        {
            //이전에 Score에 입력된 노트 모두 제거
            if (_currentMelody != null) { ChangeScore(_currentMelody, true); }

            GeneratedMelody.Clear();

            //삽입할 노트 리스트
            List<Note> notes = new List<Note>();
            int count = 0;

            //사용할 도구 객체
            MakeNote nm = new MakeNote();

            foreach (string s in files)
            {
                count++;

                notes = new MakeNote().MakeScore(s);

                GeneratedMelody.Add(new KeyValuePair<string, List<Note>>($"멜로디 {count}", notes));
            }

            CurrentMelody = GeneratedMelody[0].Key;
        }

        public void ChangeScore(string current, bool shouldRemove = false)
        {
            List<Note> notes = new List<Note>();

            foreach (KeyValuePair<string, List<Note>> i in GeneratedMelody)
            {
                if (i.Key == current)
                {
                    notes = i.Value;
                    break;
                }
            }

            if (shouldRemove)
            {
                foreach (Note i in notes)
                {
                    ViewModel.ViewModel.RemoveNote(i);
                }
            }
            else
            {
                foreach (Note i in notes)
                {
                    ViewModel.ViewModel.AddNote(i);
                }
            }
        }

        private void MelodyPlay()
        {
            if (Sequencer.IsPlaying)
            {
                Sequencer.Stop();
            }
            else
            {
                Sequencer.Continue();
            }
        }
    }
}
