using System.Linq;

namespace JUMO.UI.ViewModels
{
    public class ChopperViewModel : NoteToolsViewModel
    {
        private int _addCount;
        private int _chopInterval;

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

        public ChopperViewModel(PianoRollViewModel vm) : base(vm)
        {
            _chopInterval = 0;
            _addCount = 0;
        }

        private void Chopping(int interval)
        {
            int currentChopLength = 0;
            if (interval != 0) { currentChopLength = Song.Current.TimeResolution / interval; }

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
                        int firstLength = 0;
                        while (true)
                        {
                            if ((OrderedNotes[i][j].Start + firstLength) % currentChopLength == 0)
                            {
                                if (firstLength != 0)
                                {
                                    break;
                                }
                            }
                            firstLength++;
                        }
                        OrderedNotes[i][j].Length = firstLength;

                        int lengthHandle = OriginalNotes[i][j].Length - firstLength;
                        int startHandle = OriginalNotes[i][j].Start + firstLength;
                        while (lengthHandle > currentChopLength)
                        {
                            ViewModel.AddNote(new Note(OriginalNotes[i][j].Value, OriginalNotes[i][j].Velocity, startHandle, currentChopLength));
                            _addCount++;
                            lengthHandle -= currentChopLength;
                            startHandle += currentChopLength;
                        }
                        if (lengthHandle != 0) { ViewModel.AddNote(new Note(OriginalNotes[i][j].Value, OriginalNotes[i][j].Velocity, startHandle, lengthHandle)); _addCount++; }
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
