using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
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
                RegisterScore(score);

                return _scoreTable[score];
            }
        }

        private void RegisterScore(Score score)
        {
            if (score == null)
            {
                throw new ArgumentNullException(nameof(score));
            }

            score.CollectionChanged += OnScoreChanged;
            score.NotePropertyChanged += OnScoreNotePropertyChanged;

            _scoreTable.Add(score, new GeometryGroup() { FillRule = FillRule.Nonzero });
            RefreshScoreThumbnailGeometry(score);
        }

        private void RefreshScoreThumbnailGeometry(Score score)
        {
            GeometryCollection gc = _scoreTable[score].Children;

            gc.Clear();

            if (score == null || score.Count == 0 || score.Pattern.Length == 0)
            {
                return;
            }

            foreach (Note note in score)
            {
                gc.Add(new RectangleGeometry(new Rect(note.Start, 127 - note.Value, note.Length, 1)));
            }
        }

        private void OnScoreChanged(object sender, NotifyCollectionChangedEventArgs e) => RefreshScoreThumbnailGeometry((Score)sender);

        private void OnScoreNotePropertyChanged(object sender, EventArgs e) => RefreshScoreThumbnailGeometry((Score)sender);
    }
}
