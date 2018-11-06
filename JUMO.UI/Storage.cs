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

        #endregion

        private IEnumerable<IMusicalItem> _currenClip;
        public IEnumerable<IMusicalItem> CurrnetClip
        {
            get => _currenClip;
            set
            {
                if (_currenClip != value)
                {
                    _currenClip = value.OrderBy(note => note.Start);
                }  
            }
        }
    }
}
