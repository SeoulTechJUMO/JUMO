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
            if (_ViewModel.SelectedItems.Count() != 0)
            {
                foreach(IMusicalItem item in _ViewModel.SelectedItems)
                {
                    SelectedNotes.Add((NoteViewModel)item);
                }
            }
            else
            {
                SelectedNotes = _ViewModel.Notes;
            }
            OrderByStart();
        }

        public ObservableCollection<NoteViewModel> SelectedNotes = new ObservableCollection<NoteViewModel>();
        public Dictionary<int, List<NoteViewModel>> OrderedNoteDict = new Dictionary<int, List<NoteViewModel>>();

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

        public void OrderByStart()
        {
            var OrderedElement = SelectedNotes.OrderBy(note => note.Start);
            int currentStart = 0;
            List<NoteViewModel> tempNotes = new List<NoteViewModel>();

            foreach (NoteViewModel item in OrderedElement)
            {
                if (item == OrderedElement.ElementAt(0)) { currentStart = item.Start; }
                if (currentStart == item.Start)
                {
                    tempNotes.Add(item);
                }
                else
                {
                    OrderedNoteDict.Add(currentStart, new List<NoteViewModel>(tempNotes));
                    currentStart = item.Start;
                    tempNotes.Clear();
                    tempNotes.Add(item);
                }
                if (item == OrderedElement.ElementAt(OrderedElement.Count() - 1))
                {
                    OrderedNoteDict.Add(currentStart, new List<NoteViewModel>(tempNotes));
                }
            }
        }

        private RelayCommand _ApplyCommand;
        private RelayCommand _AbortCommand;

        public RelayCommand ApplyCommand => _ApplyCommand ?? (_ApplyCommand = new RelayCommand(_ => Apply()));
        public void Apply()
        {
            foreach (NoteViewModel item in SelectedNotes)
            {
                item.UpdateSource();
            }
        }

        public RelayCommand AbortCommand => _AbortCommand ?? (_AbortCommand = new RelayCommand(_ => Abort()));
        public void Abort()
        {
            foreach (NoteViewModel item in SelectedNotes)
            {
                item.Value = item.Source.Value;
                item.Start = item.Source.Start;
                item.Length = item.Source.Length;
                item.Velocity = item.Source.Velocity;
            }
        }
    }

    public class SofterViewModel : NoteToolsViewModel
    {
        public SofterViewModel(PianoRollViewModel vm) : base(vm)
        {
            _StartInterval = 0.0;
            _VelocityInterval = 0.0;
            _StartAdjustRange = 30;
            _VelocityAdjustRange = 30;
            _LengthAdjustRange = 30;
        }

        #region Attributes

        private int _StartAdjustRange;
        private int _VelocityAdjustRange;
        private int _LengthAdjustRange;

        private double _StartInterval;
        private double _VelocityInterval;
        private double _LengthInterval;

        #endregion

        #region Property

        public int StartAdjustRange
        {
            get => _StartAdjustRange;
            set
            {
                if (0 <= value && value <= 100)
                {
                    _StartAdjustRange = value;
                    AdjustStart(StartInterval);
                    OnPropertyChanged(nameof(StartAdjustRange));
                }
            }
        }
        public int VelocityAdjustRange
        {
            get => _VelocityAdjustRange;
            set
            {
                if (0 <= value && value <= 100)
                {
                    _VelocityAdjustRange = value;
                    AdjustVelocity(VelocityInterval);
                    OnPropertyChanged(nameof(VelocityAdjustRange));
                }
            }
        }
        public int LengthAdjustRange
        {
            get => _LengthAdjustRange;
            set
            {
                if (0 <= value && value <= 100)
                {
                    _LengthAdjustRange = value;
                    AdjustLength(LengthInterval);
                    OnPropertyChanged(nameof(LengthAdjustRange));
                }
            }
        }

        public double StartInterval
        {
            get => _StartInterval;
            set
            {
                if (-1.0 <= value && value <= 1.0)
                {
                    _StartInterval = value;
                    AdjustStart(value);
                    OnPropertyChanged(nameof(StartInterval));
                }
            }
        }
        public double VelocityInterval
        {
            get => _VelocityInterval;
            set
            {
                if (-1.0 <= value && value <= 1.0)
                {
                    _VelocityInterval = value;
                    AdjustVelocity(value);
                    OnPropertyChanged(nameof(VelocityInterval));
                }
            }
        }
        public double LengthInterval
        {
            get => _LengthInterval;
            set
            {
                if (-1.0 <= value && value <= 1.0)
                {
                    _LengthInterval = value;
                    AdjustLength(value);
                    OnPropertyChanged(nameof(LengthInterval));
                }
            }
        }

        #endregion

        private void AdjustStart(double interval)
        {
            bool IsDesc = false;
            if (interval < 0) { IsDesc = true; interval = - (interval); }
            int startDelta = 0;
            int delta = (int)(interval * StartAdjustRange);

            foreach (KeyValuePair<int, List<NoteViewModel>> item in OrderedNoteDict)
            {
                startDelta = 0;
                if (IsDesc)
                {
                    foreach (NoteViewModel note in item.Value.OrderByDescending(note=>note.Value))
                    {
                        note.Start = note.Source.Start + startDelta;
                        if (note.Start < note.Source.Start)
                        {
                            note.Start = note.Source.Start;
                        }
                        startDelta += delta;
                    }
                }
                else
                {
                    foreach (NoteViewModel note in item.Value.OrderBy(note => note.Value))
                    {
                        note.Start = item.Key + startDelta;
                        if (note.Start < item.Key)
                        {
                            note.Start = item.Key;
                        }
                        startDelta += delta;
                    }
                }
            }
        }
        private void AdjustVelocity(double interval)
        {
            bool IsDesc = false;
            if (interval < 0) { IsDesc = true; interval = -(interval); }
            int veloDelta = 0;
            int delta = (int)(interval * VelocityAdjustRange);

            foreach (KeyValuePair<int, List<NoteViewModel>> item in OrderedNoteDict)
            {
                veloDelta = 0;
                if (IsDesc)
                {
                    foreach (NoteViewModel note in item.Value.OrderByDescending(note => note.Value))
                    {
                        note.Velocity = (byte)(note.Source.Velocity - veloDelta);
                        if (note.Velocity > 127) { note.Velocity = 0; }
                        veloDelta += delta;
                    }
                }
                else
                {
                    foreach (NoteViewModel note in item.Value.OrderBy(note => note.Value))
                    {
                        note.Velocity = (byte)(note.Source.Velocity - veloDelta);
                        if (note.Velocity > 127) { note.Velocity = 0; }
                        veloDelta += delta;
                    }
                }
            }
        }
        private void AdjustLength(double interval)
        {
            bool IsDesc = false;
            if (interval < 0) { IsDesc = true; interval = -(interval); }
            int lenDelta = 0;
            int delta = (int)(interval * LengthAdjustRange);

            foreach (KeyValuePair<int, List<NoteViewModel>> item in OrderedNoteDict)
            {
                lenDelta = 0;
                if (IsDesc)
                {
                    foreach (NoteViewModel note in item.Value.OrderByDescending(note => note.Value))
                    {
                        if (note.Source.Length - lenDelta < 10) { note.Length = 10; }
                        else { note.Length = note.Source.Length - lenDelta; }
                        lenDelta += delta;
                    }
                }
                else
                {
                    foreach (NoteViewModel note in item.Value.OrderBy(note => note.Value))
                    {
                        if (note.Source.Length - lenDelta < 10) { note.Length = 10; }
                        else { note.Length = note.Source.Length - lenDelta; }
                        lenDelta += delta;
                    }
                }
            }
        }
    }

    public class ChopperViewModel : NoteToolsViewModel
    {
        public ChopperViewModel(PianoRollViewModel vm) : base(vm) { }


    }
}
