﻿using System;
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
        }

        //선택된 조성의 키값
        private string _Key;
        public string Key
        {
            get => _Key;
            set
            {
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
        private ObservableCollection<Progress> _CurrentProgress = new ObservableCollection<Progress>();
        public ObservableCollection<Progress> CurrentProgress
        {
            get => _CurrentProgress;
            set
            {
                _CurrentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
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
                    _InsertProgress = new RelayCommand(progress => InsertChord(progress as Progress), _ => progress.Any());
                }
                return _InsertProgress;        
            }
        }
        public void InsertChord(Progress chord)
        {
            if (chord != null)
            {
                CurrentProgress.Add(chord);
                progress = API.Request(chord.ChildPath);

                if (progress.Count == 0)
                {
                    MessageBox.Show("다음으로 적합한 코드진행이 없습니다.");
                }
                
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
        
        //선택 코드진행 리셋
        private RelayCommand _Reset;
        public RelayCommand Reset
        {
            get
            {
                if (_Reset == null)
                {
                    _Reset = new RelayCommand(progress => ChordReset(), _ => CurrentProgress.Any());
                }
                return _Reset;
            }
        }
        public void ChordReset()
        {
            CurrentProgress.Clear();
            progress = API.Request("");
        }

        //선택한 코드진행만 삭제
        private RelayCommand _Remove;
        public RelayCommand Remove
        {
            get
            {
                if (_Remove == null)
                {
                    _Remove = new RelayCommand(progress => ChordRemove(progress as Progress), _ => CurrentProgress.Any());
                }
                return _Remove;
            }
        }
        public void ChordRemove(Progress chord)
        {
            if (chord != null)
            {
                string cp = "";
                //현재 진행 리스트에서 선택된 객체 삭제
                CurrentProgress.Remove(chord);
                //child path 만들기
                foreach (Progress i in CurrentProgress)
                {
                    cp += i.ID;
                    i.ChildPath = cp;
                    cp += ",";
                }
                if (cp.Length > 0)
                {
                    cp = cp.Substring(0, cp.Length - 1);
                }
                progress = API.Request(cp);
            }
            else
            {
                MessageBox.Show("삭제할 코드를 선택해주세요.");
            }

        }
    }
}
