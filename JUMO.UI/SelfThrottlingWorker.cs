using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    class SelfThrottlingWorker
    {
        private readonly Func<int, int> _handler;
        private readonly int _idealDuration;
        private int _quanta;

        public SelfThrottlingWorker(int initialQuanta, int idealDuration, Func<int, int> handler)
        {
            _handler = handler;
            _quanta = initialQuanta;
            _idealDuration = idealDuration;
        }

        public void RunWorker()
        {
            Stopwatch sw = Stopwatch.StartNew();
            int count = _handler?.Invoke(_quanta) ?? 0;
            sw.Stop();

            long duration = sw.ElapsedMilliseconds;

            if (duration > 0 && count > 0)
            {
                long estimatedFullDuration = duration * (_quanta / count);
                long newQuanta = (_quanta * _idealDuration) / estimatedFullDuration;

                _quanta = Math.Max(100, (int)Math.Min(newQuanta, int.MaxValue));
            }
        }
    }
}
