using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        #region MusicalCanvasBase Members

        protected override double CalculateLogicalLength()
        {
            throw new NotImplementedException();
        }

        protected override Rect CalculateRectForElement(FrameworkElement element)
        {
            throw new NotImplementedException();
        }

        protected override Size CalculateSizeForElement(FrameworkElement element)
        {
            throw new NotImplementedException();
        }

        protected override IVirtualElement CreateVirtualElementForItem(object item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMusicalViewCallback Members

        public void MusicalViewLeftButtonDown(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewMoveComplete(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewMoveStarted(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewResizeComplete(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewResizeStarted(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewResizing(FrameworkElement view, double delta)
        {
            throw new NotImplementedException();
        }

        public void MusicalViewRightButtonDown(FrameworkElement view)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
