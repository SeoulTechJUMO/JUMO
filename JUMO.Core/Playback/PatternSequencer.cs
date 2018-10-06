using System;
using System.Collections.Generic;
using MidiToolkit = Sanford.Multimedia.Midi;
using Jacobi.Vst.Core;

namespace JUMO.Playback
{
    class PatternSequencer
    {
        private readonly List<IEnumerator<long>> _enumerators = new List<IEnumerator<long>>();
        private int _position = 0;

        #region Properties

        public Pattern Pattern { get; }

        #endregion

        public PatternSequencer(MasterSequencer masterSequencer, Pattern pattern)
        {
            if (masterSequencer == null)
            {
                throw new ArgumentNullException(nameof(masterSequencer));
            }

            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));

            masterSequencer.Tick += OnMasterClockTick;

            foreach (Vst.Plugin plugin in Vst.PluginManager.Instance.Plugins)
            {
                _enumerators.Add(GetTickIterator(Pattern[plugin].MidiTrack, plugin, 0).GetEnumerator());
            }
        }

        private IEnumerable<long> GetTickIterator(MidiToolkit.Track track, Vst.Plugin plugin, int startPosition)
        {
            IEnumerator<MidiToolkit.MidiEvent> enumerator = track.Iterator().GetEnumerator();
            List<VstEvent> simultaneousEvents = new List<VstEvent>();

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

                simultaneousEvents.Clear();

                while (hasNext && enumerator.Current.AbsoluteTicks == ticks)
                {
                    simultaneousEvents.Add(new VstMidiEvent(0, 0, 0, enumerator.Current.MidiMessage.GetBytes(), 0, 64));

                    hasNext = enumerator.MoveNext();
                }

                if (simultaneousEvents.Count > 0)
                {
                    plugin.SendEvents(simultaneousEvents.ToArray());
                }

                ticks++;
            }
        }

        private void OnMasterClockTick(object sender, EventArgs e)
        {
            _position++;

            foreach (IEnumerator<long> enumerator in _enumerators)
            {
                enumerator.MoveNext();
            }
        }
    }
}
