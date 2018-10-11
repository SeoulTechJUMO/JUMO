using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChordMagicianModel;

namespace JUMO.UI
{
    public class ChordMagicViewModel : ViewModelBase
    {
        #region Nested Types

        public enum ChangeChordResult
        {
            Success,        // 성공
            EmptyResult,    // "다음으로 적합한 코드진행이 없습니다."
            MissingInsert,  // "코드를 선택해 주세요."
            MissingRemove   // "삭제할 코드를 선택해 주세요."
        }

        public class ChordChangedEventArgs : EventArgs
        {
            public ChangeChordResult ResultCode { get; }

            public ChordChangedEventArgs(ChangeChordResult resultCode) => ResultCode = resultCode;
        }

        #endregion

        private string _key;
        private string _mode;
        private int _octave;
        private ObservableCollection<Progress> _progress;
        private ObservableCollection<Progress> _currentProgress = new ObservableCollection<Progress>();
        private Progress _currentChord;
        private bool _isClientBusy = false;

        private RelayCommand _insertProgressCommand;
        private RelayCommand _playChordCommand;
        private RelayCommand _resetCommand;
        private RelayCommand _removeCommand;
        private RelayCommand _playSelectedChordCommand;
        private RelayCommand _insertToPianoRollCommand;
        private RelayCommand _octaveDownCommand;
        private RelayCommand _octaveUpCommand;

        #region Properties

        public override string DisplayName => "코드 마법사";

        //사용중인 API Bearer
        public GetAPI API { get; }

        //현재 피아노롤 뷰모델
        public PianoRollViewModel ViewModel { get; }

        //선택된 조성의 키값
        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged(nameof(Key));
                ChangeAllChordName();
            }
        }

        //선택된 조성의 스케일
        public string Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged(nameof(Mode));
                ChangeAllChordName();
            }
        }

        //재생, 생성 노트 옥타브
        public int Octave
        {
            get => _octave;
            set
            {
                _octave = value;
                OnPropertyChanged(nameof(Octave));
            }
        }

        //받아온 코드진행 리스트
        public ObservableCollection<Progress> Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        //입력된 코드진행 리스트
        public ObservableCollection<Progress> CurrentProgress
        {
            get => _currentProgress;
            set
            {
                _currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
            }
        }

        //현재 선택중인 코드
        public Progress CurrentChord
        {
            get => _currentChord;
            set
            {
                _currentChord = value;
                OnPropertyChanged(nameof(CurrentChord));
            }
        }

        // WebClient가 사용 중인지 여부
        public bool IsClientBusy
        {
            get => _isClientBusy;
            set
            {
                _isClientBusy = value;
                OnPropertyChanged(nameof(IsClientBusy));
            }
        }

        #endregion

        #region Command Properties

        //코드 진행 삽입
        public RelayCommand InsertProgressCommand
            => _insertProgressCommand ?? (_insertProgressCommand = new RelayCommand(
                async progress => await InsertChord(progress as Progress),
                _ => Progress.Any()
            ));

        //코드 재생
        public RelayCommand PlayChordCommand
            => _playChordCommand ?? (_playChordCommand = new RelayCommand(progress => Play(progress as Progress)));

        //선택 코드진행 리셋
        public RelayCommand ResetCommand
            => _resetCommand ?? (_resetCommand = new RelayCommand(
                async progress => await ResetChords(),
                _ => CurrentProgress.Any()
            ));

        //선택한 코드진행만 삭제
        public RelayCommand RemoveCommand
            => _removeCommand ?? (_removeCommand = new RelayCommand(
                async progress => await RemoveChord(progress as Progress),
                _ => CurrentProgress.Any()
            ));

        //선택한 진행 재생
        public RelayCommand PlaySelectedChordCommand
            => _playSelectedChordCommand ?? (_playSelectedChordCommand = new RelayCommand(
                progress => ProgressPlay(),
                _ => CurrentProgress.Any()
            ));

        //코드 진행을 스코어에 삽입
        public RelayCommand InsertToPianorollCommand
            => _insertToPianoRollCommand ?? (_insertToPianoRollCommand = new RelayCommand(
                _ => MakeNote(),
                _ => CurrentProgress.Any()
            ));

        //옥타브 +,-
        public RelayCommand OctaveDownCommand
            => _octaveDownCommand ?? (_octaveDownCommand = new RelayCommand(
                _ => --Octave,
                _ => 0 < Octave
            ));

        public RelayCommand OctaveUpCommand
            => _octaveUpCommand ?? (_octaveUpCommand = new RelayCommand(
                _ => ++Octave,
                _ => 9 > Octave
            ));

        #endregion

        #region Events

        public event EventHandler<ChordChangedEventArgs> ChordChanged;

        #endregion

        public ChordMagicViewModel(string key, string mode, GetAPI api, ObservableCollection<Progress> progress, PianoRollViewModel vm)
        {
            API = api;
            ViewModel = vm;

            _key = key;
            _mode = mode;
            _progress = progress;
            _octave = 4;
            Progress = ChangeChordName(progress);
        }

        private async Task InsertChord(Progress chord)
        {
            if (chord == null)
            {
                OnChordChanged(ChangeChordResult.MissingInsert);

                return;
            }

            CurrentProgress.Add(chord);

            IsClientBusy = true;
            Progress = await Task.Run(() => API.GetProgress(chord.ChildPath));
            IsClientBusy = false;

            ChangeAllChordName();
            OnChordChanged(Progress.Count == 0 ? ChangeChordResult.EmptyResult : ChangeChordResult.Success);
        }

        private async Task RemoveChord(Progress chord)
        {
            if (chord == null)
            {
                OnChordChanged(ChangeChordResult.MissingRemove);

                return;
            }

            //현재 진행 리스트에서 선택된 객체 삭제
            CurrentProgress.Remove(chord);

            //child path 만들기
            string cp = "";

            foreach (Progress i in CurrentProgress)
            {
                cp += i.Id;
                i.ChildPath = cp;
                cp += ",";
            }

            if (cp.Length > 0)
            {
                cp = cp.Substring(0, cp.Length - 1);
            }

            IsClientBusy = true;
            Progress = await Task.Run(() => API.GetProgress(cp));
            IsClientBusy = false;

            ChangeAllChordName();
            OnChordChanged(ChangeChordResult.Success);
        }

        private async Task ResetChords()
        {
            CurrentProgress.Clear();

            IsClientBusy = true;
            Progress = await Task.Run(() => API.GetProgress(""));
            IsClientBusy = false;

            ChangeAllChordName();
        }

        // TODO: 비동기 메서드로 변환할 것!!!!!!
        private void Play(Progress p)
        {
            CurrentChord = p;

            foreach (byte i in p.ChordNotes)
            {
                //근음 추가
                if (i == p.ChordNotes[0])
                {
                    ViewModel.Plugin.NoteOn((byte)(i + 12*(Octave-1)), 100);
                }

                ViewModel.Plugin.NoteOn((byte)(i + 12 * Octave), 100);
            }

            Thread.Sleep(1000);

            foreach (byte i in p.ChordNotes)
            {
                if (i == p.ChordNotes[0])
                {
                    ViewModel.Plugin.NoteOff((byte)(i + 12 * (Octave - 1)), 100);
                }

                ViewModel.Plugin.NoteOff((byte)(i + 12 * Octave), 100);
            }
        }

        private void ProgressPlay()
        {
            foreach (Progress p in CurrentProgress)
            {
                Play(p);
            }
        }

        private void MakeNote()
        {
            long start = 0;

            foreach (Progress p in CurrentProgress)
            {
                foreach (byte i in p.ChordNotes)
                {
                    if (i == p.ChordNotes[0])
                    {
                        //근음 추가
                        ViewModel.AddNote(new Note((byte)(i + 12 * (Octave - 1)), 100, start, Song.Current.TimeResolution * 4));
                    }

                    ViewModel.AddNote(new Note((byte)(i + 12 * Octave), 100, start, Song.Current.TimeResolution * 4));
                }

                start += Song.Current.TimeResolution * 4;
            }
        }

        //컬렉션 프로퍼티 체인지 감지를 위한 코드네임 바꾸는 메소드
        private ObservableCollection<Progress> ChangeChordName(ObservableCollection<Progress> old_p)
        {
            ObservableCollection<Progress> new_p = new ObservableCollection<Progress>();

            foreach (Progress i in old_p)
            {
                new_p.Add(ChordTools.GetChordName(i, Key, Mode));
            }

            return new_p;
        }

        //조성이 바뀔시에 모든 코드이름 업데이트
        private void ChangeAllChordName()
        {
            if (Progress != null)
            {
                Progress = ChangeChordName(Progress);
            }

            if (CurrentProgress != null)
            {
                CurrentProgress = ChangeChordName(CurrentProgress);
            }
        }

        private void OnChordChanged(ChangeChordResult resultCode)
            => ChordChanged?.Invoke(this, new ChordChangedEventArgs(resultCode));
    }
}
