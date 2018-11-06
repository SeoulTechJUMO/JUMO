using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    public sealed class Storage
    {
        #region Singleton

        private static readonly Lazy<Storage> _instance = new Lazy<Storage>(() => new Storage());

        public static Storage Instance => _instance.Value;

        private Storage() { }

        #endregion

        private IEnumerable<IMusicalItem> _currentClip;

        public IEnumerable<IMusicalItem> CurrentClip
        {
            get => _currentClip;
            set
            {
                if (_currentClip != value)
                {
                    _currentClip = value.OrderBy(note => note.Start);
                }  
            }
        }
    }
}
