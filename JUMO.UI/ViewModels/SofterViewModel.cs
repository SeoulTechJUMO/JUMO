using System.Linq;

namespace JUMO.UI.ViewModels
{
    public class SofterViewModel : NoteToolsViewModel
    {
        public override string DisplayName => "부드럽게";

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

            for(int i=0;i<OrderedNotes.Count();i++)
            {
                startDelta = 0;
                if (IsDesc)
                {
                    for(int j = OrderedNotes[i].Count() - 1; j >= 0; j--)
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
