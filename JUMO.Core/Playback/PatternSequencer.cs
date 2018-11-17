using System;
using System.Collections.Generic;
using MidiToolkit = Sanford.Multimedia.Midi;

namespace JUMO.Playback
{
    class PatternSequencer : IDisposable
    {
        private readonly MasterSequencer _masterSequencer;
        private readonly List<IEnumerator<int>> _enumerators = new List<IEnumerator<int>>();
        private readonly int _length;
        private int _numOfPlayingScores = 0;
        private int _position = 0;

        #region Properties

        public PatternPlacement PlacedPattern { get; }
        public Pattern Pattern { get; }

        #endregion

        public PatternSequencer(MasterSequencer masterSequencer, PatternPlacement placedPattern, int startPosition)
        {
            _masterSequencer = masterSequencer ?? throw new ArgumentNullException(nameof(masterSequencer));
            _length = placedPattern.Length;
            PlacedPattern = placedPattern ?? throw new ArgumentNullException(nameof(placedPattern));

            _masterSequencer.Tick += OnMasterClockTick;
            _masterSequencer.Stopped += OnMasterSequencerStopped;

            Pattern pattern = placedPattern.Pattern;

            foreach (Vst.Plugin plugin in Vst.PluginManager.Instance.Plugins)
            {
                _enumerators.Add(GetTickIterator(pattern[plugin].MidiTrack, plugin, startPosition).GetEnumerator());
                _numOfPlayingScores++;
            }
        }

        private IEnumerable<int> GetTickIterator(MidiToolkit.Track track, Vst.Plugin plugin, int startPosition)
        {
            IEnumerator<MidiToolkit.MidiEvent> enumerator = track.Iterator().GetEnumerator();

            bool hasNext;

            for (hasNext = enumerator.MoveNext();
                 hasNext && enumerator.Current.AbsoluteTicks < startPosition;
                 hasNext = enumerator.MoveNext()) { }

            int ticks = startPosition;

            while (hasNext)
            {
                while (ticks < enumerator.Current.AbsoluteTicks)
                {
                    yield return ticks;

                    ticks++;
                }

                yield return ticks;

                while (hasNext && enumerator.Current.AbsoluteTicks == ticks)
                {
                    if (enumerator.Current.MidiMessage is MidiToolkit.ChannelMessage cm)
                    {
                        if (cm.Command == MidiToolkit.ChannelCommand.NoteOn)
                        {
                            _masterSequencer.SendNoteOn(plugin, (byte)cm.Data1, (byte)cm.Data2);
                        }
                        else if (cm.Command == MidiToolkit.ChannelCommand.NoteOff)
                        {
                            _masterSequencer.SendNoteOff(plugin, (byte)cm.Data1);
                        }
                    }

                    hasNext = enumerator.MoveNext();
                }

                ticks++;
            }

            _numOfPlayingScores--;
        }

        private void OnMasterClockTick(object sender, EventArgs e)
        {
            if (_numOfPlayingScores <= 0)
            {
                Dispose();

                return;
            }

            _position++;

            foreach (IEnumerator<int> enumerator in _enumerators)
            {
                enumerator.MoveNext();
            }
        }

        private void OnMasterSequencerStopped(object sender, EventArgs e) => Dispose();

        public void Dispose()
        {
            _masterSequencer.Tick -= OnMasterClockTick;
            _enumerators.Clear();
        }
    }
}
