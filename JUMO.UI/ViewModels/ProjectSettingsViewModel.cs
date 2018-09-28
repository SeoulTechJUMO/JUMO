using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    public class ProjectSettingsViewModel : SettingsGroupViewModel
    {
        private double _tempo = Song.Current.Tempo;
        private int _tempoBeat = Song.Current.TempoBeat;

        public override string DisplayName => "프로젝트 정보";

        public string Title { get; set; } = Song.Current.Title;
        public string Artist { get; set; } = Song.Current.Artist;
        public string Genre { get; set; } = Song.Current.Genre;
        public string Description { get; set; } = Song.Current.Description;

        public double Tempo
        {
            get => _tempo;
            set
            {
                _tempo = value;
                OnPropertyChanged(nameof(Tempo));
            }
        }

        public int TempoBeat
        {
            get => _tempoBeat;
            set
            {
                int oldValue = _tempoBeat;

                _tempoBeat = value;
                Tempo = Tempo * value / oldValue;
                OnPropertyChanged(nameof(TempoBeat));
            }
        }

        public int Numerator { get; set; } = Song.Current.Numerator;
        public int Denominator { get; set; } = Song.Current.Denominator;

        public override void SaveSettings()
        {
            // TODO: 입력된 값의 유효성을 검사할 것

            Song.Current.Title = Title;
            Song.Current.Artist = Artist;
            Song.Current.Genre = Genre;
            Song.Current.Description = Description;
            Song.Current.Numerator = Numerator;
            Song.Current.Denominator = Denominator;

            Song.Current.SetTempo(TempoBeat, Tempo);
        }
    }
}
