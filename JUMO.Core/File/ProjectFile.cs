using System;

namespace JUMO.File
{
    [Serializable]
    class ProjectFile
    {
        public Song Song { get; set; }
        public Plugin[] Plugins { get; set; }
        public string[] TrackNames { get; set; }
        public Pattern[] Patterns { get; set; }
        public PatternPlacement[] PlacedPatterns { get; set; }
        public Score[] Scores { get; set; }
    }
}
