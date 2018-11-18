using System;
using System.Collections.Generic;
using System.Linq;

namespace JUMO.UI
{
    public sealed class ClipboardService
    {
        #region Singleton

        private static readonly Lazy<ClipboardService> _instance = new Lazy<ClipboardService>(() => new ClipboardService());

        public static ClipboardService Instance => _instance.Value;

        private ClipboardService() { }

        #endregion

        public Type CurrentType { get; private set; } = typeof(object);

        public IEnumerable<IMusicalItem> CurrentItems { get; private set; }

        public void PutItems(Type typeId, IEnumerable<IMusicalItem> items)
        {
            CurrentType = typeId ?? throw new ArgumentNullException();
            CurrentItems = items.ToList();
        }
    }
}
