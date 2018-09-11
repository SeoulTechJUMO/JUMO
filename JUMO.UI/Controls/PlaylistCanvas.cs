﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JUMO.UI.Data;
using JUMO.UI.Views;

namespace JUMO.UI.Controls
{
    class PlaylistCanvas : MusicalCanvasBase, IMusicalViewCallback
    {
        private const double FOLLOW_ACCEL = 0.0625;

        private readonly BlockSelectionHelper _selectionHelper;

        #region Dependency Properties

        public static DependencyProperty SnapToGridProperty =
            DependencyProperty.Register(
                "SnapToGrid", typeof(bool), typeof(PlaylistCanvas),
                new FrameworkPropertyMetadata(true)
            );

        public bool SnapToGrid
        {
            get => (bool)GetValue(SnapToGridProperty);
            set => SetValue(SnapToGridProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<PlacePatternRequestedEventArgs> PlacePatternRequested;
        public event EventHandler<RemovePatternRequestedEventArgs> RemovePatternRequested;

        #endregion

        public PlaylistCanvas()
        {
            _selectionHelper = new BlockSelectionHelper(this);
        }

        #region MusicalCanvasBase Members

        protected override IVirtualElement CreateVirtualElementForItem(object item)
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                Point pt = e.GetPosition(this);
                Pattern pattern = Song.Current.CurrentPattern;
                int trackIndex = (int)(pt.Y / 60);
                long start = PixelToTick(pt.X);
                long snap = SnapToGridInternal(start);

                PlacePatternRequested?.Invoke(this, new PlacePatternRequestedEventArgs(pattern, trackIndex, snap));
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ClearSelection();
                _selectionHelper.StartBlockSelection(e.GetPosition(this));
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                _selectionHelper.StartBlockSelection(e.GetPosition(this));
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_selectionHelper.IsBlockSelecting)
            {
                Rect selectionRect = _selectionHelper.EndBlockSelection();

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
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                _selectionHelper.UpdateBlockSelection(e.GetPosition(this));
                FollowMouse();
            }
        }

        private long PixelToTick(double xPos) => (long)(xPos / WidthPerTick);

        private long SnapToGridInternal(long pos)
        {
            if (!SnapToGrid)
            {
                return pos;
            }

            int ticksPerBeat = (TimeResolution << 2) / MusicalProps.GetDenominator(this);
            int ticksPerGrid = ticksPerBeat / GridStep;
            return (pos / ticksPerGrid) * ticksPerGrid;
        }

        #region IMusicalViewCallback Members

        public void MusicalViewMoveStarted(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewMoveComplete(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewResizeStarted(FrameworkElement view) { }
        public void MusicalViewResizeComplete(FrameworkElement view) { }
        public void MusicalViewResizing(FrameworkElement view, double delta) { }

        public void MusicalViewLeftButtonDown(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewRightButtonDown(FrameworkElement view)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                RemovePatternRequested?.Invoke(this, new RemovePatternRequestedEventArgs((PatternPlacement)view.DataContext));
            }
        }

        #endregion

        private void FollowMouse()
        {
            Point pos = Mouse.GetPosition(this) - new Vector(HorizontalOffset, VerticalOffset);

            if (pos.X > ViewportWidth)
            {
                SetHorizontalOffset(HorizontalOffset + (pos.X - ViewportWidth) * FOLLOW_ACCEL);
            }
            else if (pos.X < 0)
            {
                SetHorizontalOffset(HorizontalOffset + pos.X * FOLLOW_ACCEL);
            }

            if (pos.Y > ViewportHeight)
            {
                SetVerticalOffset(VerticalOffset + (pos.Y - ViewportHeight) * FOLLOW_ACCEL);
            }
            else if (pos.Y < 0)
            {
                SetVerticalOffset(VerticalOffset + pos.Y * FOLLOW_ACCEL);
            }
        }
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
