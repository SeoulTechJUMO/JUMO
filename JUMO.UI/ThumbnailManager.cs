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

        private readonly Dictionary<Score, GeometryGroup> _scoreTable = new Dictionary<Score, GeometryGroup>();

        private ThumbnailManager() { }

        public GeometryGroup GetThumbnailForScore(Score score)
        {
            if (_scoreTable.TryGetValue(score, out GeometryGroup geometry))
            {
                return geometry;
            }
            else
            {
                GeometryGroup newGeometry = new GeometryGroup() { FillRule = FillRule.Nonzero };
                _scoreTable.Add(score, newGeometry);

                return newGeometry;
            }
        }
    }
}
