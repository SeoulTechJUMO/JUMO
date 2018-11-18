using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JUMO.UI.ViewModels
{
    public abstract class NoteToolsViewModel : ViewModelBase
    {
        private readonly ObservableCollection<NoteViewModel> _selectedNotes = new ObservableCollection<NoteViewModel>();

        private RelayCommand _abortCommand;

        public struct OldNotes
        {
            public byte Value;
            public byte Velocity;
            public int Start;
            public int Length;

            public OldNotes(byte value, byte velocity, int start, int length)
            {
                Value = value;
                Velocity = velocity;
                Start = start;
                Length = length;
            }
        }

        public List<List<NoteViewModel>> OrderedNotes = new List<List<NoteViewModel>>();
        public List<List<OldNotes>> OriginalNotes = new List<List<OldNotes>>();
        public bool WillInsert;

        public PianoRollViewModel ViewModel { get; }

        public RelayCommand AbortCommand => _abortCommand ?? (_abortCommand = new RelayCommand(Reset));

        public NoteToolsViewModel(PianoRollViewModel vm)
        {
            ViewModel = vm;
            WillInsert = false;
            if (ViewModel.SelectedItems.Count() != 0)
            {
                foreach (IMusicalItem item in ViewModel.SelectedItems)
                {
                    _selectedNotes.Add((NoteViewModel)item);
                }
            }
            else
            {
                _selectedNotes = ViewModel.Notes;
            }
            OrderByStart();
        }

        //시작점으로 그룹화, 노트 피치 순으로 정렬
        private void OrderByStart()
        {
            var orderedElements = _selectedNotes.OrderBy(note => note.Start);
            int currentStart = orderedElements.Select(note => note.Start).FirstOrDefault();
            List<NoteViewModel> tempNotes = new List<NoteViewModel>();
            List<OldNotes> tempOld = new List<OldNotes>();

            foreach (NoteViewModel item in orderedElements)
            {
                if (currentStart == item.Start)
                {
                    tempNotes.Add(item);
                    tempOld.Add(new OldNotes(item.Value, item.Velocity, item.Start, item.Length));
                }
                else
                {
                    OrderedNotes.Add(new List<NoteViewModel>(tempNotes.OrderBy(note => note.Value).ToList()));
                    OriginalNotes.Add(new List<OldNotes>(tempOld.OrderBy(note => note.Value).ToList()));

                    currentStart = item.Start;

                    tempNotes.Clear();
                    tempOld.Clear();

                    tempNotes.Add(item);
                    tempOld.Add(new OldNotes(item.Value, item.Velocity, item.Start, item.Length));
                }

                if (item == orderedElements.ElementAt(orderedElements.Count() - 1))
                {
                    OrderedNotes.Add(new List<NoteViewModel>(tempNotes.OrderBy(note => note.Value).ToList()));
                    OriginalNotes.Add(new List<OldNotes>(tempOld.OrderBy(note => note.Value).ToList()));
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < OrderedNotes.Count(); i++)
            {
                for (int j = 0; j < OrderedNotes[i].Count(); j++)
                {
                    OrderedNotes[i][j].Value = OriginalNotes[i][j].Value;
                    OrderedNotes[i][j].Velocity = OriginalNotes[i][j].Velocity;
                    OrderedNotes[i][j].Start = OriginalNotes[i][j].Start;
                    OrderedNotes[i][j].Length = OriginalNotes[i][j].Length;
                    OrderedNotes[i][j].UpdateSource();
                }
            }
        }
    }
}
