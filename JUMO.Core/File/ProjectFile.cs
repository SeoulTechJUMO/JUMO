using System;

namespace JUMO.File
{
    [Serializable]
    class ProjectFile
    {
        public Song Song { get; set; }
        public Plugin[] Plugins { get; set; }
    }
}
