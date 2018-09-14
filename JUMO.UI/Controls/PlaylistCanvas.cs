﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JUMO.UI.Data;
using JUMO.UI.Views;

namespace JUMO.UI.Controls
{
    class PlaylistCanvas : InteractiveMusicalCanvas
    {
        #region Events

        public event EventHandler<PlacePatternRequestedEventArgs> PlacePatternRequested;
        public event EventHandler<RemovePatternRequestedEventArgs> RemovePatternRequested;

        #endregion

        public PlaylistCanvas() : base() { }

        #region MusicalCanvasBase Members

        protected override IVirtualElement CreateVirtualElementForItem(IMusicalItem item)
        {
            return new VirtualPatternControl((PatternPlacement)item);
        }

        protected override double CalculateLogicalLength() => Song.Current.Length + (TimeResolution << 6);

        protected override Size CalculateSizeForElement(FrameworkElement element)
        {
            PatternPlacement pp = (PatternPlacement)element.DataContext;

            return new Size(pp.Length * WidthPerTick, 60);
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            PatternPlacement pp = (PatternPlacement)element.DataContext;

            return new Rect(new Point(pp.Start * WidthPerTick, pp.TrackIndex * 60), new Size(pp.Length * WidthPerTick, 60));
        }

        #endregion

        protected override void OnSurfaceClick(Point pt)
        {
            Pattern pattern = Song.Current.CurrentPattern;
            int trackIndex = (int)(pt.Y / 60);
            long start = PixelToTick(pt.X);
            long snap = SnapToGridInternal(start);

            PlacePatternRequested?.Invoke(this, new PlacePatternRequestedEventArgs(pattern, trackIndex, snap));
        }

        protected override void OnBlockSelectionCompleted(Rect selectionRect)
        {
            long startTick = Math.Max(0L, PixelToTick(selectionRect.Left));
            long length = PixelToTick(selectionRect.Width);
            int lowIndex = (int)Math.Max(0, Math.Min(selectionRect.Top / 60, 63));
            int highIndex = (int)Math.Max(0, Math.Min(selectionRect.Bottom / 60, 63));

            var selectedPatterns =
                GetVirtualElementsInside(new Segment(startTick, length))
                .Select(ve => (PatternPlacement)((PatternPlacementView)ve.Visual).DataContext)
                .Where(pp => pp.TrackIndex >= lowIndex && pp.TrackIndex <= highIndex);

            SelectItems(new List<PatternPlacement>(selectedPatterns));
        }

        #region IMusicalViewCallback Members

        public override void MusicalViewMoveStarted(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public override void MusicalViewMoveComplete(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public override void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY)
        {
            throw new NotImplementedException();
        }

        public override void MusicalViewLeftButtonDown(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public override void MusicalViewRightButtonDown(FrameworkElement view)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                RemovePatternRequested?.Invoke(this, new RemovePatternRequestedEventArgs((PatternPlacement)view.DataContext));
            }
        }

        #endregion
    }

    class PlacePatternRequestedEventArgs : EventArgs
    {
        public Pattern Pattern { get; }
        public int TrackIndex { get; }
        public long Start { get; }

        public PlacePatternRequestedEventArgs(Pattern pattern, int trackIndex, long start)
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
