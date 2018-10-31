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
        private readonly Dictionary<Pattern, GeometryGroup> _patternTable = new Dictionary<Pattern, GeometryGroup>();

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

        public Geometry GetThumbnailForPattern(Pattern pattern)
        {
            if (_patternTable.TryGetValue(pattern, out GeometryGroup geometry))
            {
                return geometry;
            }
            else
            {
                RegisterPattern(pattern);

                return _patternTable[pattern];
            }
        }

        #region Internal Methods for Score Thumbnails

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

            foreach (Note note in score)
            {
                tempGeometry.Children.Add(new RectangleGeometry(new Rect(note.Start, 127 - note.Value, note.Length, 1)));
            }

            _scoreTable[score].Figures = tempGeometry.GetFlattenedPathGeometry().Figures;
        }

        private void OnScoreChanged(object sender, NotifyCollectionChangedEventArgs e) => RefreshScoreThumbnailGeometry((Score)sender);

        private void OnScoreNotePropertyChanged(object sender, EventArgs e) => RefreshScoreThumbnailGeometry((Score)sender);

        #endregion

        #region Internal Methods for Pattern Thumbnails

        private void RegisterPattern(Pattern pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            pattern.ScoreChanged += OnPatternScoreChanged;

            _patternTable.Add(pattern, new GeometryGroup() { FillRule = FillRule.Nonzero });
            RefreshPatternThumbnailGeometry(pattern);
        }

        private void RefreshPatternThumbnailGeometry(Pattern pattern)
        {
            GeometryGroup geometry = _patternTable[pattern];

            geometry.Children.Clear();

            foreach (Score score in pattern.Scores)
            {
                geometry.Children.Add(GetThumbnailForScore(score));
            }
        }

        private void OnPatternScoreChanged(object sender, ScoreChangedEventArgs e)
        {
            if (e.RemovedScores != null)
            {
                foreach (Score score in e.RemovedScores)
                {
                    _scoreTable.Remove(score);
                }
            }

            RefreshPatternThumbnailGeometry((Pattern)sender);
        }

        #endregion
    }
}
