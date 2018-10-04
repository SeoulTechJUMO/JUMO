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
                new CreateMelody().RunMagenta(Chord);
                ProgressVisible = Visibility.Hidden;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
