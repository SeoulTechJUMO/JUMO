using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ChordMagicianTest;
using System.Windows.Input;
using System.Windows;
using JUMO.UI;

namespace ChordMagicianTest.ViewModel
{
    public class ChordMagicViewModel : INotifyPropertyChanged
    {
        public ChordMagicViewModel(string key, string mode, getAPI API, ObservableCollection<Progress> progress)
        {
            _Key = key;
            _Mode = mode;
            _API = API;
            _progress = progress;
            _MakeProgress = null;
        }

        //선택된 조성의 키값
        private string _Key;
        public string Key
        {
            get => _Key;
            set
            {
                _Key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        //선택된 조성의 스케일
        private string _Mode;
        public string Mode
        {
            get => _Mode;
            set
            {
                _Mode = value;
                OnPropertyChanged(nameof(Mode));
            }
        }

        //사용중인 API Bearer
        private getAPI _API;
        public getAPI API
        {
            get => _API;
            set
            {
                _API = value;
                OnPropertyChanged(nameof(API));
            }
        }

        //받아온 코드진행 리스트
        private ObservableCollection<Progress> _progress;
        public ObservableCollection<Progress> progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(progress));
            }
        }

        //입력된 코드진행 리스트
        private ObservableCollection<Progress> _MakeProgress;
        public ObservableCollection<Progress> MakeProgress
        {
            get => _MakeProgress;
            set
            {
                _MakeProgress = value;
                OnPropertyChanged(nameof(MakeProgress));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //사용 커맨드
        private RelayCommand _InsertProgress;
        public RelayCommand InsertProgress
        {
            get
            {
                if (_InsertProgress == null)
                {
                    _InsertProgress = new RelayCommand(progress => InsertChord(progress as Progress));
                }
                return _InsertProgress;        
            }
        }
        public void InsertChord(Progress chord)
        {
            if (chord != null)
            {
                progress = API.Request(chord.ChildPath);
            }
            else
            {
                MessageBox.Show("코드를 선택해주세요.");
            }
        }

        private RelayCommand _PlayChord;
        public RelayCommand PlayChord
        {
            get
            {
                if (_PlayChord == null)
                {
                    _PlayChord = new RelayCommand(progress => Play(progress as Progress));
                }
                return _PlayChord;
            }
        }
        public void Play(Progress p)
        {
            //TODO:미디메시지를 생성해서 VST에 전송해야됨
            System.Diagnostics.Debug.WriteLine(p);
        }
    }
}
