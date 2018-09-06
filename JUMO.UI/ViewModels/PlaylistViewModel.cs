using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    public class PlaylistViewModel : WorkspaceViewModel
    {
        public override WorkspaceKey Key => PlaylistWorkspaceKey.Instance;

        public override string DisplayName { get; } = "플레이리스트";
    }
}
