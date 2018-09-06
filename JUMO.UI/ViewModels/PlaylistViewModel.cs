using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    public class PlaylistViewModel : MusicalCanvasWorkspaceViewModel
    {
        protected override double ZoomBase => 4.0;

        public override WorkspaceKey Key => PlaylistWorkspaceKey.Instance;
        public override string DisplayName { get; } = "플레이리스트";

        public override IEnumerable<int> GridStepOptions { get; } = new[] { 1, 2, 4 };
    }
}
