using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JUMO.File
{
    public class ProjectReader
    {
        private const uint Version = 0;

        private static readonly byte[] MagicBytes = new byte[4] { 75, 73, 65, 126 };

        private readonly JUMO.Song _song = JUMO.Song.Current;
        private readonly Vst.PluginManager _pluginManager = Vst.PluginManager.Instance;
        private readonly MixerManager _mixerManager = MixerManager.Instance;

        public void LoadFile(string path)
        {
            try
            {
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    bool magicOK = CheckMagic(stream);
                    bool versionOK = CheckVersion(stream);

                    if (magicOK && versionOK)
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        ProjectFile file = (ProjectFile)formatter.Deserialize(stream);

                        DoLoadFile(file);
                    }
                    else
                    {
                        // Fail
                    }
                }
            }
            catch
            {
                // Fail
            }
        }

        private bool CheckMagic(Stream stream)
        {
            byte[] buf = new byte[4];

            if (stream.Read(buf, 0, 4) == 4)
            {
                return buf[0] == MagicBytes[0] && buf[1] == MagicBytes[1] && buf[2] == MagicBytes[2] && buf[3] == MagicBytes[3];
            }

            return false;
        }

        private bool CheckVersion(Stream stream)
        {
            byte[] buf = new byte[4];

            if (stream.Read(buf, 0, 4) == 4)
            {
                uint parsedVersion = BitConverter.ToUInt32(buf, 0);

                return parsedVersion == Version;
            }

            return false;
        }

        private void DoLoadFile(ProjectFile file)
        {
            file.Song.Restore(_song);

            IDictionary<int, Vst.Plugin> pluginTable = RestorePlugins(file);
            IDictionary<int, JUMO.Pattern> patternTable = RestorePatterns(file);
            RestoreTrackNames(file);
            RestoreScores(file, patternTable, pluginTable);
            RestorePatternPlacements(file, patternTable);
            RestoreMixerChannels(file);

            _song.CurrentPattern = _song.Patterns[0];
        }

        private IDictionary<int, Vst.Plugin> RestorePlugins(ProjectFile file)
        {
            Dictionary<int, Vst.Plugin> pluginTable = new Dictionary<int, Vst.Plugin>();

            _pluginManager.UnloadAll();

            foreach (Plugin plugin in file.Plugins)
            {
                Vst.Plugin p = _pluginManager.AddPlugin(plugin.PluginPath, null);

                if (p == null)
                {
                    // Fail? Open a OpenFileDialog?
                }
                else
                {
                    pluginTable.Add(plugin.Id, p);
                    plugin.Restore(p);
                }
            }

            return pluginTable;
        }

        private IDictionary<int, JUMO.Pattern> RestorePatterns(ProjectFile file)
        {
            Dictionary<int, JUMO.Pattern> patternTable = new Dictionary<int, JUMO.Pattern>();

            _song.Patterns.Clear();

            foreach (Pattern pattern in file.Patterns)
            {
                JUMO.Pattern p = new JUMO.Pattern(_song, pattern.Name);

                patternTable.Add(pattern.Id, p);
                _song.Patterns.Add(p);
            }

            return patternTable;
        }

        private void RestoreScores(ProjectFile file, IDictionary<int, JUMO.Pattern> patternTable, IDictionary<int, Vst.Plugin> pluginTable)
        {
            foreach (Score score in file.Scores)
            {
                JUMO.Pattern pattern = patternTable[score.PatternId];

                if (pluginTable.TryGetValue(score.PluginId, out Vst.Plugin plugin))
                {
                    score.Restore(pattern[plugin]);
                }
                else
                {
                    // This score is for a plugin JUMO has failed to load.

                    continue;
                }
            }
        }

        private void RestoreTrackNames(ProjectFile file)
        {
            int trackCount = Math.Min(file.TrackNames.Length, _song.Tracks.Length);

            for (int i = 0; i < trackCount; i++)
            {
                _song.Tracks[i].Name = file.TrackNames[i];
            }
        }

        private void RestorePatternPlacements(ProjectFile file, IDictionary<int, JUMO.Pattern> patternTable)
        {
            _song.PlacedPatterns.Clear();

            foreach (PatternPlacement pp in file.PlacedPatterns)
            {
                _song.PlacedPatterns.Add(new JUMO.PatternPlacement(patternTable[pp.PatternId], pp.TrackId, pp.Start));
                // Cannot set the Length property for now.
            }
        }

        private void RestoreMixerChannels(ProjectFile file)
        {
            foreach (JUMO.MixerChannel ch in _mixerManager.MixerChannels)
            {
                ch.UnloadAllEffects();
            }

            int chCount = Math.Min(_mixerManager.MixerChannels.Length, file.MixerChannels.Length);

            for (int i = 0; i < chCount; i++)
            {
                MixerChannel source = file.MixerChannels[i];
                JUMO.MixerChannel target = _mixerManager.MixerChannels[i];

                target.Panning = source.Panning;
                target.Volume = source.Volume;
                target.IsMuted = source.IsMuted;

                foreach (EffectPlugin plugin in source.Plugins)
                {
                    Vst.EffectPlugin p = target.AddEffect(plugin.PluginPath);

                    if (p == null)
                    {
                        // Fail? Open a OpenFileDialog?
                    }
                    else
                    {
                        plugin.Restore(p);
                    }
                }
            }
        }
    }
}
