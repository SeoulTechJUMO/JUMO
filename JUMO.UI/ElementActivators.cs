using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JUMO.UI.Controls;

namespace JUMO.UI
{
    delegate IVirtualElement VirtualElementActivator(object arg);

    static class ElementActivators
    {
        public static VirtualElementActivator VirtualNoteActivator { get; }

        static ElementActivators()
        {
            // VirtualNoteActivator
            Expression<VirtualElementActivator> newVirtualNoteExpr = (object arg) => new VirtualNote(arg as INote);
            VirtualNoteActivator = newVirtualNoteExpr.Compile();
        }
    }
}
