using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace JUMO.UI
{
    class ThumbnailManager
    {
        #region Singleton

        private static readonly Lazy<ThumbnailManager> _instance = new Lazy<ThumbnailManager>(() => new ThumbnailManager());

        public static ThumbnailManager Instance => _instance.Value;

        #endregion

        private readonly Dictionary<Score, GeometryGroup> _table = new Dictionary<Score, GeometryGroup>();

        public GeometryGroup this[Score score]
        {
            get
            {
                if (_table.TryGetValue(score, out GeometryGroup geometry))
                {
                    return geometry;
                }
                else
                {
                    GeometryGroup newGeometry = new GeometryGroup() { FillRule = FillRule.Nonzero };
                    _table.Add(score, newGeometry);

                    return newGeometry;
                }
            }
        }

        private ThumbnailManager() { }
    }
}
