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

        private readonly Dictionary<Score, PathGeometry> _scoreTable = new Dictionary<Score, PathGeometry>();

        private ThumbnailManager() { }

        public Geometry GetThumbnailForScore(Score score)
        {
            if (_scoreTable.TryGetValue(score, out PathGeometry geometry))
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

            _scoreTable.Add(score, new PathGeometry() { FillRule = FillRule.Nonzero });
            RefreshScoreThumbnailGeometry(score);
        }

        private void RefreshScoreThumbnailGeometry(Score score)
        {
            GeometryGroup tempGeometry = new GeometryGroup() { FillRule = FillRule.Nonzero };
            GeometryCollection gc = tempGeometry.Children;

            foreach (Note note in score)
            {
                gc.Add(new RectangleGeometry(new Rect(note.Start, 127 - note.Value, note.Length, 1)));
            }

            _scoreTable[score].Figures = tempGeometry.GetFlattenedPathGeometry().Figures;
        }

        private void OnScoreChanged(object sender, NotifyCollectionChangedEventArgs e) => RefreshScoreThumbnailGeometry((Score)sender);

        private void OnScoreNotePropertyChanged(object sender, EventArgs e) => RefreshScoreThumbnailGeometry((Score)sender);
    }
}
