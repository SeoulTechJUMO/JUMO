using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JUMO.UI.Controls
{
    class PlaylistCanvas : MusicalCanvasBase, IMusicalViewCallback
    {
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
                throw new NotImplementedException();
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                throw new NotImplementedException();
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
