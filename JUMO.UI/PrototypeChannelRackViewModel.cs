using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUMO.Vst;

namespace JUMO.UI
{
    class PrototypeChannelRackViewModel
    {
        public IEnumerable<Plugin> Plugins { get; } = PluginManager.Instance.Plugins;
    }
}
