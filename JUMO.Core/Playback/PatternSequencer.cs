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
                    System.Diagnostics.Debug.WriteLine($"{Pattern.Name}[{plugin.Name}]: Ticks = {ticks}, Message = {string.Join(", ", enumerator.Current.MidiMessage.GetBytes())}");

                    hasNext = enumerator.MoveNext();
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
