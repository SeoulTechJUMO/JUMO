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
                    AdjustStart(StartInterval);
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
                    AdjustVelocity(VelocityInterval);
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
                    AdjustLength(LengthInterval);
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
                    AdjustStart(value);
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
                    AdjustVelocity(value);
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
                    AdjustLength(value);
                    OnPropertyChanged(nameof(LengthInterval));
                }
            }
        }

        #endregion

        public SofterViewModel(PianoRollViewModel vm) : base(vm) { }

        private void AdjustStart(double interval)
        {
            bool IsDesc = false;

            if (interval < 0)
            {
                IsDesc = true;
                interval = -interval;
            }

            int delta = (int)(interval * StartAdjustRange);
            var zipped = OrderedNotes.Zip(OriginalNotes, (group, origGroup) => (group, origGroup));

            foreach ((var group, var origGroup) in zipped)
            {
                IEnumerable<NoteViewModel> g = IsDesc ? ((IEnumerable<NoteViewModel>)group).Reverse() : group;
                IEnumerable<Note> og = IsDesc ? ((IEnumerable<Note>)origGroup).Reverse() : origGroup;
                int startDelta = 0;
                var z = g.Zip(og, (note, origNote) => (note, origNote));

                foreach((NoteViewModel note, Note origNote) in z)
                {
                    note.Start = Math.Max(origNote.Start + startDelta, origNote.Start);
                    startDelta += delta;

                    note.UpdateSource();
                }
            }
        }

        private void AdjustVelocity(double interval)
        {
            bool IsDesc = false;

            if (interval < 0)
            {
                IsDesc = true;
                interval = -interval;
            }

            int delta = (int)(interval * VelocityAdjustRange);
            var zipped = OrderedNotes.Zip(OriginalNotes, (group, origGroup) => (group, origGroup));

            foreach ((var group, var origGroup) in zipped)
            {
                IEnumerable<NoteViewModel> g = IsDesc ? ((IEnumerable<NoteViewModel>)group).Reverse() : group;
                IEnumerable<Note> og = IsDesc ? ((IEnumerable<Note>)origGroup).Reverse() : origGroup;
                int veloDelta = 0;
                var z = g.Zip(og, (note, origNote) => (note, origNote));

                foreach ((NoteViewModel note, Note origNote) in z)
                {
                    note.Velocity = (byte)(origNote.Velocity - veloDelta);

                    if (note.Velocity > 127)
                    {
                        note.Velocity = 0;
                    }

                    veloDelta += delta;

                    note.UpdateSource();
                }
            }
        }

        private void AdjustLength(double interval)
        {
            bool IsDesc = false;

            if (interval < 0)
            {
                IsDesc = true;
                interval = -interval;
            }

            int delta = (int)(interval * LengthAdjustRange);
            var zipped = OrderedNotes.Zip(OriginalNotes, (group, origGroup) => (group, origGroup));

            foreach ((var group, var origGroup) in zipped)
            {
                IEnumerable<NoteViewModel> g = IsDesc ? ((IEnumerable<NoteViewModel>)group).Reverse() : group;
                IEnumerable<Note> og = IsDesc ? ((IEnumerable<Note>)origGroup).Reverse() : origGroup;
                int lenDelta = 0;
                var z = g.Zip(og, (note, origNote) => (note, origNote));

                foreach ((NoteViewModel note, Note origNote) in z)
                {
                    note.Length = Math.Max(origNote.Length - lenDelta, 10);
                    lenDelta += delta;

                    note.UpdateSource();
                }
            }
        }
    }
}
