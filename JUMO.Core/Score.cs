using System;
using System.Collections.ObjectModel;

namespace JUMO
{
    public class Score : ObservableCollection<Note>
    {
        public Pattern Pattern { get; }

        public Score(Pattern pattern)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        }
    }
}
