using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JUMO.UI
{
    public class PianoRollViewModel : MusicalCanvasWorkspaceViewModel<Note>
    {
        protected override double ZoomBase => 24.0;

        public override WorkspaceKey Key { get; }
        public override string DisplayName => $"피아노 롤: {Plugin.Name}";

        public Vst.Plugin Plugin { get; }
        public Pattern Pattern => Song.Current.CurrentPattern;

        public override IEnumerable<int> GridStepOptions { get; } = new[] { 1, 2, 3, 4, 6, 8, 12, 16 };

        public Score Notes => Pattern[Plugin];

        public PianoRollViewModel(Vst.Plugin plugin) : base()
        {
            Plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            Key = new PianoRollWorkspaceKey(plugin);

            Song.Current.PropertyChanged += CurrentSong_PropertyChanged;

            GridStep = Denominator >= 4 ? 4 : 2;
        }

        private void CurrentSong_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);

            if (e.PropertyName.Equals(nameof(Song.CurrentPattern)))
            {
                OnPropertyChanged(nameof(Notes));
            }
        }
    }
}
