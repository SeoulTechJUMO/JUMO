using System;

namespace JUMO.File
{
    [Serializable]
    class Score
    {
        public int PatternId { get; }
        public int PluginId { get; }
        public Note[] Notes { get; }

        public Score(JUMO.Score source, int patternId, int pluginId)
        {
            PatternId = patternId;
            PluginId = pluginId;

            int noteCount = source.Count;
            int noteIndex = 0;
            Notes = new Note[noteCount];

            foreach (JUMO.Note note in source)
            {
                Notes[noteIndex++] = new Note(note);
            }
        }
    }
}
