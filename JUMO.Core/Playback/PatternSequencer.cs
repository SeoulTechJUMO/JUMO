using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiToolkit = Sanford.Multimedia.Midi;

namespace JUMO.Playback
{
    class PatternSequencer
    {
        #region Properties

        public Pattern Pattern { get; }

        #endregion

        public PatternSequencer(/* MasterSequencer? x?, */ Pattern pattern)
        {
            Pattern = pattern;
        }

        public IEnumerable<long> GetTickIterator(MidiToolkit.Track track, Vst.Plugin plugin, int startPosition)
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
                    System.Diagnostics.Debug.WriteLine($"Ticks = {ticks}, Message = {string.Join(", ", enumerator.Current.MidiMessage.GetBytes())}");

                    hasNext = enumerator.MoveNext();
                }

                ticks++;
            }
        }
    }
}
