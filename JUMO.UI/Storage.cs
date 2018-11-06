using System;
using System.Collections.Generic;
using System.Linq;

namespace JUMO.UI
{
    public sealed class Storage
    {
        #region Singleton

        private static readonly Lazy<Storage> _instance = new Lazy<Storage>(() => new Storage());

        public static Storage Instance => _instance.Value;

        private Storage() { }

        #endregion

        public Type CurrentType { get; private set; }

        public IEnumerable<IMusicalItem> CurrentClip { get; private set; }

        public void PutItems(Type typeId, IEnumerable<IMusicalItem> items)
        {
            CurrentType = typeId ?? throw new ArgumentNullException();
            CurrentClip = items.OrderBy(item => item.Start).ToList();
        }
    }
}
