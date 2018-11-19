using System.Linq;

namespace JUMO.UI.ViewModels
{
    public class ChopperViewModel : NoteToolsViewModel
    {
        private int _addCount = 0;
        private int _chopInterval = 0;

        #region Properties

        public override string DisplayName => "자르기";

        public int ChopInterval
        {
            get => _chopInterval;
            set
            {
                if (value != _chopInterval)
                {
                    if (value >= 0 && value <= 16 && value % 2 == 0)
                    {
                        _chopInterval = value;
                        Chopping(value);
                        OnPropertyChanged(nameof(ChopInterval));
                    }
                }
            }
        }

        #endregion

        public ChopperViewModel(PianoRollViewModel vm) : base(vm) { }

        private void Chopping(int interval)
        {
            int currentChopLength = 0;

            if (interval != 0)
            {
                currentChopLength = Song.Current.TimeResolution / interval;
            }

            for (int i = 0; i < _addCount; i++)
            {
                ViewModel.RemoveNote(ViewModel.Notes?.ElementAt(ViewModel.Notes.Count - 1).Source);
            }

            _addCount = 0;

            if (currentChopLength == 0)
            {
                Reset();
            }
            else
            {
                for (int i = 0; i < OrderedNotes.Count(); i++)
                {
                    for (int j = 0; j < OrderedNotes[i].Count(); j++)
                    {
                        NoteViewModel note = OrderedNotes[i][j];
                        Note origNote = OriginalNotes[i][j];
                        int firstLength = (note.Start / currentChopLength + 1) * currentChopLength - note.Start;

                        note.Length = firstLength;
                        note.UpdateSource();

                        int lengthHandle = origNote.Length - firstLength;
                        int startHandle = origNote.Start + firstLength;

                        while (lengthHandle > currentChopLength)
                        {
                            ViewModel.AddNote(new Note(origNote.Value, origNote.Velocity, startHandle, currentChopLength));
                            _addCount++;
                            lengthHandle -= currentChopLength;
                            startHandle += currentChopLength;
                        }

                        if (lengthHandle != 0)
                        {
                            ViewModel.AddNote(new Note(origNote.Value, origNote.Velocity, startHandle, lengthHandle));
                            _addCount++;
                        }
                    }
                }
            }
        }

        public void ChopperReset()
        {
            Chopping(0);
        }
    }
}
