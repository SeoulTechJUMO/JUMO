using System.Linq;

namespace JUMO.UI.ViewModels
{
    public class SofterViewModel : NoteToolsViewModel
    {
        private int _startAdjustRange;
        private int _velocityAdjustRange;
        private int _lengthAdjustRange;

        private double _startInterval;
        private double _velocityInterval;
        private double _lengthInterval;

        #region Properties

        public override string DisplayName => "부드럽게";

        public int StartAdjustRange
        {
            get => _startAdjustRange;
            set
            {
                if (0 <= value && value <= 100)
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
                if (0 <= value && value <= 100)
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
                if (0 <= value && value <= 100)
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
                if (-1.0 <= value && value <= 1.0)
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
                if (-1.0 <= value && value <= 1.0)
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
                if (-1.0 <= value && value <= 1.0)
                {
                    _lengthInterval = value;
                    AdjustLength(value);
                    OnPropertyChanged(nameof(LengthInterval));
                }
            }
        }

        #endregion

        public SofterViewModel(PianoRollViewModel vm) : base(vm)
        {
            _startInterval = 0.0;
            _velocityInterval = 0.0;
            _lengthInterval = 0.0;
            _startAdjustRange = 30;
            _velocityAdjustRange = 30;
            _lengthAdjustRange = 30;
        }

        private void AdjustStart(double interval)
        {
            bool IsDesc = false;
            if (interval < 0) { IsDesc = true; interval = -(interval); }
            int startDelta = 0;
            int delta = (int)(interval * StartAdjustRange);

            for (int i = 0; i < OrderedNotes.Count(); i++)
            {
                startDelta = 0;
                if (IsDesc)
                {
                    for (int j = OrderedNotes[i].Count() - 1; j >= 0; j--)
                    {
                        OrderedNotes[i][j].Start = OriginalNotes[i][j].Start + startDelta;
                        if (OrderedNotes[i][j].Start < OriginalNotes[i][j].Start)
                        {
                            OrderedNotes[i][j].Start = OriginalNotes[i][j].Start;
                        }
                        startDelta += delta;
                        OrderedNotes[i][j].UpdateSource();
                    }
                }
                else
                {
                    for (int j = 0; j < OrderedNotes[i].Count(); j++)
                    {
                        OrderedNotes[i][j].Start = OriginalNotes[i][j].Start + startDelta;
                        if (OrderedNotes[i][j].Start < OriginalNotes[i][j].Start)
                        {
                            OrderedNotes[i][j].Start = OriginalNotes[i][j].Start;
                        }
                        startDelta += delta;
                        OrderedNotes[i][j].UpdateSource();
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

            for (int i = 0; i < OrderedNotes.Count(); i++)
            {
                veloDelta = 0;
                if (IsDesc)
                {
                    for (int j = OrderedNotes[i].Count() - 1; j >= 0; j--)
                    {
                        OrderedNotes[i][j].Velocity = (byte)(OriginalNotes[i][j].Velocity - veloDelta);
                        if (OrderedNotes[i][j].Velocity > 127) { OrderedNotes[i][j].Velocity = 0; }
                        veloDelta += delta;
                        OrderedNotes[i][j].UpdateSource();
                    }
                }
                else
                {
                    for (int j = 0; j < OrderedNotes[i].Count(); j++)
                    {
                        OrderedNotes[i][j].Velocity = (byte)(OriginalNotes[i][j].Velocity - veloDelta);
                        if (OrderedNotes[i][j].Velocity > 127) { OrderedNotes[i][j].Velocity = 0; }
                        veloDelta += delta;
                        OrderedNotes[i][j].UpdateSource();
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

            for (int i = 0; i < OrderedNotes.Count(); i++)
            {
                lenDelta = 0;
                if (IsDesc)
                {
                    for (int j = OrderedNotes[i].Count() - 1; j >= 0; j--)
                    {
                        if (OriginalNotes[i][j].Length - lenDelta < 10) { OrderedNotes[i][j].Length = 10; }
                        else { OrderedNotes[i][j].Length = OriginalNotes[i][j].Length - lenDelta; }
                        lenDelta += delta;
                        OrderedNotes[i][j].UpdateSource();
                    }
                }
                else
                {
                    for (int j = 0; j < OrderedNotes[i].Count(); j++)
                    {
                        if (OriginalNotes[i][j].Length - lenDelta < 10) { OrderedNotes[i][j].Length = 10; }
                        else { OrderedNotes[i][j].Length = OriginalNotes[i][j].Length - lenDelta; }
                        lenDelta += delta;
                        OrderedNotes[i][j].UpdateSource();
                    }
                }
            }
        }
    }
}
