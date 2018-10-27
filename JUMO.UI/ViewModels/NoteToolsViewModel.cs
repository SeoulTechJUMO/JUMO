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
        public struct OldNotes
        {
            byte Value;
            byte Velocity;
            int Start;
            int Length;

            public OldNotes(byte value, byte velocity, int start, int length)
            {
                Value = value;
                Velocity = velocity;
                Start = start;
                Length = length;
            }
        }

        public override string DisplayName => $"노트 편집 도구";

        private ObservableCollection<NoteViewModel> SelectedNotes = new ObservableCollection<NoteViewModel>();
        private PianoRollViewModel _ViewModel;

        private RelayCommand _ApplyCommand;
        private RelayCommand _AbortCommand;

        public List<List<NoteViewModel>> OrderedNotes = new List<List<NoteViewModel>>();
        public List<List<OldNotes>> OriginalNotes = new List<List<OldNotes>>();
        public bool WillInsert;

        public PianoRollViewModel ViewModel
        {
            get => _ViewModel;
            set
            {
                _ViewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }

        public RelayCommand ApplyCommand => _ApplyCommand ?? (_ApplyCommand = new RelayCommand(_ => Apply()));
        public RelayCommand AbortCommand => _AbortCommand ?? (_AbortCommand = new RelayCommand(_ => Reset()));

        public NoteToolsViewModel(PianoRollViewModel vm)
        {
            _ViewModel = vm;
            WillInsert = false;
            if (_ViewModel.SelectedItems.Count() != 0)
            {
                foreach (IMusicalItem item in _ViewModel.SelectedItems)
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

        private void OrderByStart()
        {
            var OrderedElement = SelectedNotes.OrderBy(note => note.Start);
            int currentStart = 0;
            List<NoteViewModel> tempNotes = new List<NoteViewModel>();
            List<OldNotes> tempOld = new List<OldNotes>();

            foreach (NoteViewModel item in OrderedElement)
            {
                if (item == OrderedElement.ElementAt(0)) { currentStart = item.Start; }
                if (currentStart == item.Start)
                {
                    tempNotes.Add(item);
                    tempOld.Add(new OldNotes(item.Value,item.Velocity,item.Start,item.Length));
                }
                else
                {
                    OrderedNotes.Add(new List<NoteViewModel>(tempNotes));
                    OriginalNotes.Add(tempOld);

                    currentStart = item.Start;

                    tempNotes.Clear();
                    tempOld.Clear();

                    tempNotes.Add(item);
                    tempOld.Add(new OldNotes(item.Value, item.Velocity, item.Start, item.Length));
                }
                if (item == OrderedElement.ElementAt(OrderedElement.Count() - 1))
                {
                    OrderedNotes.Add(new List<NoteViewModel>(tempNotes));
                    OriginalNotes.Add(tempOld);
                }
            }
        }

        public void Apply()
        {
            foreach (NoteViewModel item in SelectedNotes)
            {
                item.UpdateSource();
            }
        }

        public void Reset()
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
            _LengthInterval = 0.0;
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

            foreach (List<NoteViewModel> item in OrderedNotes)
            {
                startDelta = 0;
                if (IsDesc)
                {
                    foreach (NoteViewModel note in item.OrderByDescending(note=>note.Value))
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
                    foreach (NoteViewModel note in item.OrderBy(note => note.Value))
                    {
                        note.Start = note.Source.Start + startDelta;
                        if (note.Start < note.Source.Start)
                        {
                            note.Start = note.Source.Start;
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

            foreach (List<NoteViewModel> item in OrderedNotes)
            {
                veloDelta = 0;
                if (IsDesc)
                {
                    foreach (NoteViewModel note in item.OrderByDescending(note => note.Value))
                    {
                        note.Velocity = (byte)(note.Source.Velocity - veloDelta);
                        if (note.Velocity > 127) { note.Velocity = 0; }
                        veloDelta += delta;
                    }
                }
                else
                {
                    foreach (NoteViewModel note in item.OrderBy(note => note.Value))
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

            foreach (List<NoteViewModel> item in OrderedNotes)
            {
                lenDelta = 0;
                if (IsDesc)
                {
                    foreach (NoteViewModel note in item.OrderByDescending(note => note.Value))
                    {
                        if (note.Source.Length - lenDelta < 10) { note.Length = 10; }
                        else { note.Length = note.Source.Length - lenDelta; }
                        lenDelta += delta;
                    }
                }
                else
                {
                    foreach (NoteViewModel note in item.OrderBy(note => note.Value))
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
        public ChopperViewModel(PianoRollViewModel vm) : base(vm)
        {
            _ChopInterval = 0;
        }



        private int _ChopInterval;
        public int ChopInterval
        {
            get => _ChopInterval;
            set
            {
                if (value >= 0)
                {
                    _ChopInterval = value;
                    Chopping(value);
                    OnPropertyChanged(nameof(ChopInterval));
                }
            }
        }

        private int _ChopLength;
        public int ChopLength
        {
            get => _ChopLength;
            set
            {
                if (value >= 0)
                {
                    _ChopLength = value;
                    LengthAdjust(value);
                    OnPropertyChanged(nameof(ChopLength));
                }
            }
        }

        private void Chopping(int interval)
        {
            int currentChopLength = 0;
            if (interval != 0) { currentChopLength = Song.Current.TimeResolution / interval; }

            foreach (List<NoteViewModel> group in OrderedNotes)
            {
                foreach (NoteViewModel note in group)
                {
                    
                }
            }
        }

        private void LengthAdjust(int length)
        {

        }
    }
}
