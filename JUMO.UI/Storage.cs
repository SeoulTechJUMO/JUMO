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

        public Type CurrentType { get; private set; } = typeof(object);

        public IEnumerable<IMusicalItem> CurrentClip { get; private set; }

        public void PutItems(Type typeId, IEnumerable<IMusicalItem> items)
        {
            CurrentType = typeId ?? throw new ArgumentNullException();
            CurrentClip = items.ToList();
        }
    }
}
