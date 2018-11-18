using System;
using System.Collections.Generic;
using System.Linq;

namespace JUMO.UI.ViewModels
{
    public class SofterViewModel : NoteToolsViewModel
    {
        private const int MinRange = 0;
        private const int MaxRange = 100;
        private const double MinInterval = -1.0;
        private const double MaxInterval = 1.0;

        private int _startAdjustRange = 30;
        private int _velocityAdjustRange = 30;
        private int _lengthAdjustRange = 30;
        private double _startInterval = 0.0;
        private double _velocityInterval = 0.0;
        private double _lengthInterval = 0.0;

        #region Properties

        public override string DisplayName => "부드럽게";

        public int StartAdjustRange
        {
            get => _startAdjustRange;
            set
            {
                if (MinRange <= value && value <= MaxRange)
                {
                    _startAdjustRange = value;
                    AdjustValue(StartInterval, value, DoAdjustStart);
                    OnPropertyChanged(nameof(StartAdjustRange));
                }
            }
        }

        public int VelocityAdjustRange
        {
            get => _velocityAdjustRange;
            set
            {
                if (MinRange <= value && value <= MaxRange)
                {
                    _velocityAdjustRange = value;
                    AdjustValue(VelocityInterval, value, DoAdjustVelocity);
                    OnPropertyChanged(nameof(VelocityAdjustRange));
                }
            }
        }

        public int LengthAdjustRange
        {
            get => _lengthAdjustRange;
            set
            {
                if (MinRange <= value && value <= MaxRange)
                {
                    _lengthAdjustRange = value;
                    AdjustValue(LengthInterval, value, DoAdjustLength);
                    OnPropertyChanged(nameof(LengthAdjustRange));
                }
            }
        }

        public double StartInterval
        {
            get => _startInterval;
            set
            {
                if (MinInterval <= value && value <= MaxInterval)
                {
                    _startInterval = value;
                    AdjustValue(value, StartAdjustRange, DoAdjustStart);
                    OnPropertyChanged(nameof(StartInterval));
                }
            }
        }

        public double VelocityInterval
        {
            get => _velocityInterval;
            set
            {
                if (MinInterval <= value && value <= MaxInterval)
                {
                    _velocityInterval = value;
                    AdjustValue(value, VelocityAdjustRange, DoAdjustVelocity);
                    OnPropertyChanged(nameof(VelocityInterval));
                }
            }
        }

        public double LengthInterval
        {
            get => _lengthInterval;
            set
            {
                if (MinInterval <= value && value <= MaxInterval)
                {
                    _lengthInterval = value;
                    AdjustValue(value, LengthAdjustRange, DoAdjustLength);
                    OnPropertyChanged(nameof(LengthInterval));
                }
            }
        }

        #endregion

        public SofterViewModel(PianoRollViewModel vm) : base(vm) { }

        private void AdjustValue(double interval, int adjustRange, Action<NoteViewModel, Note, int> transform)
        {
            bool isDesc = false;

            if (interval < 0)
            {
                isDesc = true;
                interval = -interval;
            }

            int delta = (int)(interval * adjustRange);
            var zipped = OrderedNotes.Zip(OriginalNotes, (group, origGroup) => (group, origGroup));

            foreach ((var group, var origGroup) in zipped)
            {
                IEnumerable<NoteViewModel> g = isDesc ? ((IEnumerable<NoteViewModel>)group).Reverse() : group;
                IEnumerable<Note> og = isDesc ? ((IEnumerable<Note>)origGroup).Reverse() : origGroup;
                int valueDelta = 0;
                var z = g.Zip(og, (note, origNote) => (note, origNote));

                foreach ((NoteViewModel note, Note origNote) in z)
                {
                    transform(note, origNote, valueDelta);
                    note.UpdateSource();

                    valueDelta += delta;
                }
            }
        }

        private void DoAdjustStart(NoteViewModel note, Note origNote, int delta)
        {
            note.Start = Math.Max(origNote.Start + delta, origNote.Start);
        }

        private void DoAdjustVelocity(NoteViewModel note, Note origNote, int delta)
        {
            note.Velocity = (byte)(origNote.Velocity - delta);

            if (note.Velocity > 127)
            {
                note.Velocity = 0;
            }
        }

        private void DoAdjustLength(NoteViewModel note, Note origNote, int delta)
        {
            note.Length = Math.Max(origNote.Length - delta, 10);
        }
    }
}
