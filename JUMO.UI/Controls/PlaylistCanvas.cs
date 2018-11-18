using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JUMO.UI.Data;
using JUMO.UI.ViewModels;
using JUMO.UI.Views;

namespace JUMO.UI.Controls
{
    class PlaylistCanvas : InteractiveMusicalCanvas
    {
        private IEnumerable<PatternPlacementViewModel> _affectedItems;
        private PatternPlacementViewModel _minTick;
        private PatternPlacementViewModel _minTrack, _maxTrack;

        #region Events

        public event EventHandler<PlacePatternRequestedEventArgs> PlacePatternRequested;
        public event EventHandler<RemovePatternRequestedEventArgs> RemovePatternRequested;

        #endregion

        public PlaylistCanvas() : base() { }

        #region MusicalCanvasBase Members

        protected override IVirtualElement CreateVirtualElementForItem(IMusicalItem item)
        {
            return new VirtualPatternControl((PatternPlacementViewModel)item);
        }

        protected override double CalculateLogicalLength() => Song.Current.Length + (TimeResolution << 6);

        protected override Size CalculateSizeForElement(FrameworkElement element)
        {
            PatternPlacementViewModel pp = (PatternPlacementViewModel)element.DataContext;

            return new Size(pp.Length * WidthPerTick, 60);
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            PatternPlacementViewModel pp = (PatternPlacementViewModel)element.DataContext;

            return new Rect(new Point(pp.Start * WidthPerTick, pp.TrackIndex * 60), new Size(pp.Length * WidthPerTick, 60));
        }

        #endregion

        #region InteractiveMusicalCanvas Overrides

        protected override int MinVerticalValue => 0;
        protected override int MaxVerticalValue => 63;

        protected override int GetVerticalValue(IMusicalItem item) => ((PatternPlacementViewModel)item).TrackIndex;
        protected override int FromVerticalPosition(double y) => (int)y / 60;

        protected override void OnSurfaceClick(Point pt)
        {
            Pattern pattern = Song.Current.CurrentPattern;
            int trackIndex = FromVerticalPosition(pt.Y);
            int start = PixelToTick(pt.X);
            int snap = SnapToGridInternal(start);

            PlacePatternRequested?.Invoke(this, new PlacePatternRequestedEventArgs(pattern, trackIndex, snap));
        }

        #endregion

        #region IMusicalViewCallback Members

        public override void MusicalViewResizeStarted(FrameworkElement view) => CalculateAffectedItems(view);
        public override void MusicalViewMoveStarted(FrameworkElement view) => CalculateAffectedItems(view);
        public override void MusicalViewResizeComplete(FrameworkElement view) => ViewEditComplete(view);
        public override void MusicalViewMoveComplete(FrameworkElement view) => ViewEditComplete(view);

        public override void MusicalViewResizing(FrameworkElement view, double delta)
        {
            foreach (PatternPlacementViewModel pp in _affectedItems)
            {
                ResizePattern(pp, delta);
            }

            FollowMouse();
        }

        public override void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY)
        {
            int deltaStart = PixelToTick(deltaX);
            int deltaIndex = FromVerticalDelta(deltaY);

            if (deltaX < 0 && _minTick.Start < -deltaStart)
            {
                deltaStart = -_minTick.Start;
            }

            if (deltaY > 0 && MaxVerticalValue - _maxTrack.TrackIndex < deltaIndex)
            {
                deltaIndex = MaxVerticalValue - _maxTrack.TrackIndex;
            }

            if (deltaY < 0 && MinVerticalValue - _minTick.TrackIndex > deltaIndex)
            {
                deltaIndex = MinVerticalValue - _minTick.TrackIndex;
            }

            foreach (PatternPlacementViewModel pp in _affectedItems)
            {
                MovePattern(pp, deltaStart, deltaIndex);
            }

            FollowMouse();
        }

        public override void MusicalViewLeftButtonDown(FrameworkElement view)
        {
            PatternPlacementView ppView = (PatternPlacementView)view;

            if (Keyboard.Modifiers == ModifierKeys.None)
            {
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ClearSelection();
                SelectItem(ppView.DataContext);
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                if (ppView.IsSelected)
                {
                    DeselectItem(ppView.DataContext);
                }
                else
                {
                    SelectItem(ppView.DataContext);
                }
            }
        }

        public override void MusicalViewRightButtonDown(FrameworkElement view)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                PatternPlacement pp = ((PatternPlacementViewModel)view.DataContext).Source;
                RemovePatternRequested?.Invoke(this, new RemovePatternRequestedEventArgs(pp));
            }
        }

        #endregion

        private void CalculateAffectedItems(FrameworkElement view)
        {
            if (((PatternPlacementView)view).IsSelected)
            {
                _affectedItems = SelectedItems.Cast<PatternPlacementViewModel>();
                _minTick = _affectedItems.MinBy(pp => pp.Start);
                (_minTrack, _maxTrack) = _affectedItems.MinMaxBy(pp => pp.TrackIndex);
            }
            else
            {
                _affectedItems = new[] { (PatternPlacementViewModel)view.DataContext };
                _minTick = _minTrack = _maxTrack = (PatternPlacementViewModel)view.DataContext;
            }
        }

        private void ViewEditComplete(FrameworkElement view)
        {
            foreach (PatternPlacementViewModel pp in _affectedItems)
            {
                pp.UpdateSource();
            }

            _affectedItems = null;
            LastPressedItem = (PatternPlacementViewModel)view.DataContext;
        }

        private void ResizePattern(PatternPlacementViewModel pp, double delta)
        {
            int end = pp.Start + pp.Length;
            int newEnd = SnapToGridInternal(end + PixelToTick(delta));

            if (newEnd > pp.Start)
            {
                pp.Length = newEnd - pp.Start;
            }
        }

        private void MovePattern(PatternPlacementViewModel pp, int deltaStart, int deltaIndex)
        {
            int newStart = SnapToGridInternal(pp.Start + deltaStart);
            int newIndex = pp.TrackIndex + deltaIndex;

            pp.Start = Math.Max(0, newStart);
            pp.TrackIndex = Math.Max(MinVerticalValue, Math.Min(newIndex, MaxVerticalValue));
        }
    }

    class PlacePatternRequestedEventArgs : EventArgs
    {
        public Pattern Pattern { get; }
        public int TrackIndex { get; }
        public int Start { get; }

        public PlacePatternRequestedEventArgs(Pattern pattern, int trackIndex, int start)
        {
            Pattern = pattern;
            TrackIndex = trackIndex;
            Start = start;
        }
    }

    class RemovePatternRequestedEventArgs : EventArgs
    {
        public PatternPlacement PatternToRemove { get; }

        public RemovePatternRequestedEventArgs(PatternPlacement patternToRemove)
        {
            PatternToRemove = patternToRemove;
        }
    }
}
