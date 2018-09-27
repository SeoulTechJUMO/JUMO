using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    public class ProjectSettingsViewModel : SettingsGroupViewModel
    {
        public override string DisplayName => "프로젝트 정보";

        public string Title { get; set; } = Song.Current.Title;
        public string Artist { get; set; } = Song.Current.Artist;
        public string Genre { get; set; } = Song.Current.Genre;
        public string Description { get; set; } = Song.Current.Description;

        public int Tempo { get; set; } = Song.Current.Tempo;

        public int Numerator { get; set; } = Song.Current.Numerator;
        public int Denominator { get; set; } = Song.Current.Denominator;

        public override void SaveSettings()
        {
            // TODO: 입력된 값의 유효성을 검사할 것

            Song.Current.Title = Title;
            Song.Current.Artist = Artist;
            Song.Current.Genre = Genre;
            Song.Current.Description = Description;
            Song.Current.Tempo = Tempo;
            Song.Current.Numerator = Numerator;
            Song.Current.Denominator = Denominator;
        }
    }
}
