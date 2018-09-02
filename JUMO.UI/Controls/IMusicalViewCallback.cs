using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JUMO.UI.Controls
{
    interface IMusicalViewCallback
    {
        void MusicalViewResizeStarted(FrameworkElement view);
        void MusicalViewResizing(FrameworkElement view, double delta);
        void MusicalViewResizeComplete(FrameworkElement view);
        void MusicalViewMoveStarted(FrameworkElement view);
        void MusicalViewMoving(FrameworkElement view, double deltaX, double deltaY);
        void MusicalViewMoveComplete(FrameworkElement view);
        void MusicalViewLeftButtonDown(FrameworkElement view);
        void MusicalViewRightButtonDown(FrameworkElement view);
    }
}
