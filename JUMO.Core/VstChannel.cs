using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jacobi.Vst.Core.Host;

namespace JUMO
{
    [Obsolete("JUMO.Vst.Plugin으로 대체될 예정")]
    class VstChannel
    {
        public string Name { get; set; }
        public bool IsMuted { get; set; } // TODO
        public bool IsSolo { get; set; } // TODO
        public int Insert { get; set; } // TODO
        public IVstPluginCommandStub Plugin { get; set; } // TODO

        public VstChannel(IVstPluginCommandStub plugCmdStub, int insert)
        {
            Plugin = plugCmdStub;

            Name = Plugin.GetEffectName();
            IsMuted = IsSolo = false;
            Insert = insert;
        }
    }
}
