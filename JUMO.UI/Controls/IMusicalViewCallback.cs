using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI.Controls
{
    interface IMusicalViewCallback
    {
        void MusicalViewResizing(object musicalObject, double delta);
        void MusicalViewResizeComplete(object musicalObject);
        void MusicalViewMoving(object musicalObject, double deltaX, double deltaY);
        void MusicalViewMoveComplete(object musicalObject);
    }
}
