using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JUMO.UI.ViewModels
{
    public abstract class NoteToolsViewModel : ViewModelBase
    {
        private readonly ObservableCollection<NoteViewModel> _selectedNotes = new ObservableCollection<NoteViewModel>();

        private RelayCommand _abortCommand;

        protected readonly List<List<NoteViewModel>> OrderedNotes = new List<List<NoteViewModel>>();
        protected readonly List<List<Note>> OriginalNotes = new List<List<Note>>();

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
            var sortedAndGrouped =
                from note in _selectedNotes
                orderby note.Start, note.Value
                group note by note.Start;

            foreach (var group in sortedAndGrouped)
            {
                OrderedNotes.Add(group.ToList());
                OriginalNotes.Add(group.Select(note => new Note(note.Value, note.Velocity, note.Start, note.Length)).ToList());
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
