using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JUMO.File
{
    public class ProjectWriter : IProjectWriter
    {
        public const uint MinimumSupportedVersion = 0;
        public const uint MaximumSupportedVersion = 1;

        private static readonly byte[] MagicBytes = new byte[4] { 75, 73, 65, 126 };
        private static readonly byte[] VersionData = new byte[4] { 0, 0, 0, 0 };

        private readonly JUMO.Song _song = JUMO.Song.Current;
        private readonly Vst.PluginManager _pluginManager = Vst.PluginManager.Instance;
        private readonly MixerManager _mixerManager = MixerManager.Instance;

        public static bool SaveFile(string path)
        {
            return new V1.ProjectWriter().DoSaveFile(path);
        }

        public bool DoSaveFile(string path)
        {
            ProjectFile file = PrepareFile();
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(MagicBytes, 0, 4);
                    stream.Write(VersionData, 0, 4);
                    formatter.Serialize(stream, file);
                }

                _song.FilePath = path;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private ProjectFile PrepareFile()
        {
            ProjectFile file = new ProjectFile
            {
                Song = new Song(_song)
            };

            IDictionary<Vst.Plugin, int> pluginTable = FillPluginEntries(file);
            IDictionary<JUMO.Pattern, int> patternTable = FillPatterns(file, pluginTable);
            FillTrackNames(file);
            FillPatternPlacements(file, patternTable);
            FillMixerChannels(file);

            return file;
        }

        private IDictionary<Vst.Plugin, int> FillPluginEntries(ProjectFile file)
        {
            Dictionary<Vst.Plugin, int> pluginTable = new Dictionary<Vst.Plugin, int>();
            int pluginCount = _pluginManager.Plugins.Count;
            int pluginIndex = 0;
            file.Plugins = new Plugin[pluginCount];

            foreach (Vst.Plugin plugin in _pluginManager.Plugins)
            {
                pluginTable.Add(plugin, pluginIndex);
                file.Plugins[pluginIndex] = new Plugin(pluginIndex, plugin);

                pluginIndex++;
            }

            return pluginTable;
        }

        private IDictionary<JUMO.Pattern, int> FillPatterns(ProjectFile file, IDictionary<Vst.Plugin, int> pluginTable)
        {
            Dictionary<JUMO.Pattern, int> patternTable = new Dictionary<JUMO.Pattern, int>();
            List<Score> scores = new List<Score>();
            int patternCount = _song.Patterns.Count;
            int patternIndex = 0;
            file.Patterns = new Pattern[patternCount];

            foreach (JUMO.Pattern pattern in _song.Patterns)
            {
                patternTable.Add(pattern, patternIndex);
                file.Patterns[patternIndex] = new Pattern(patternIndex, pattern);

                foreach (Vst.Plugin plugin in _pluginManager.Plugins)
                {
                    scores.Add(new Score(pattern[plugin], patternIndex, pluginTable[plugin]));
                }

                patternIndex++;
            }

            file.Scores = scores.ToArray();

            return patternTable;
        }

        private void FillTrackNames(ProjectFile file)
        {
            int trackCount = _song.Tracks.Length;
            file.TrackNames = new string[trackCount];

            for (int i = 0; i < trackCount; i++)
            {
                file.TrackNames[i] = _song.Tracks[i].Name;
            }
        }

        private void FillPatternPlacements(ProjectFile file, IDictionary<JUMO.Pattern, int> patternTable)
        {
            int ppCount = _song.PlacedPatterns.Count;
            int ppIndex = 0;
            file.PlacedPatterns = new PatternPlacement[ppCount];

            foreach (JUMO.PatternPlacement pp in _song.PlacedPatterns)
            {
                file.PlacedPatterns[ppIndex++] = new PatternPlacement(patternTable[pp.Pattern], pp);
            }
        }

        private void FillMixerChannels(ProjectFile file)
        {
            int chCount = MixerManager.NumOfMixerChannels;
            file.MixerChannels = new MixerChannel[chCount];

            for (int i = 0; i < chCount; i++)
            {
                file.MixerChannels[i] = new MixerChannel(_mixerManager.MixerChannels[i]);
            }
        }
    }
}
