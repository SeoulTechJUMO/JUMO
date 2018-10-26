using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI.ViewModels
{
    public class NoteToolsViewModel : ViewModelBase
    {
        public override string DisplayName => $"노트 편집 도구";

        public NoteToolsViewModel(PianoRollViewModel vm)
        {
            _ViewModel = vm;
            OldScore = vm.Score;
        }

        private readonly Score OldScore;

        private PianoRollViewModel _ViewModel;
        public PianoRollViewModel ViewModel
        {
            get => _ViewModel;
            set
            {
                _ViewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }

        private RelayCommand _AbortApplyCommand;
        public RelayCommand AbortApplyCommand => _AbortApplyCommand ?? (_AbortApplyCommand = new RelayCommand(_ => AbortApply()));
        public void AbortApply()
        {
            ViewModel.ReplaceScore(OldScore);
        }
    }

    public class SofterViewModel : NoteToolsViewModel
    {
        public SofterViewModel(PianoRollViewModel vm) : base(vm)
        {
            _StartInterval = 0.0;
            _VelocityInterval = 0.0;
        }

        private readonly int ADJUST_RANGE = 100;

        private double _StartInterval;
        public double StartInterval
        {
            get => _StartInterval;
            set
            {
                _StartInterval = value;
                OnPropertyChanged(nameof(StartInterval));
            }
        }

        private double _VelocityInterval;
        public double VelocityInterval
        {
            get => _VelocityInterval;
            set
            {
                _VelocityInterval = value;
                OnPropertyChanged(nameof(VelocityInterval));
            }
        }
    }

    public class ChopperViewModel : NoteToolsViewModel
    {
        public ChopperViewModel(PianoRollViewModel vm) : base(vm) { }


    }
}
