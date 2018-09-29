using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JUMO.UI.Views
{
    public partial class ScoreThumbnailView : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty ScoreProperty =
            DependencyProperty.Register(
                "Score", typeof(Score), typeof(ScoreThumbnailView),
                new FrameworkPropertyMetadata(ScorePropertyChangedCallback)
            );

        #endregion

        #region Properties

        public Score Score
        {
            get => (Score)GetValue(ScoreProperty);
            set => SetValue(ScoreProperty, value);
        }

        #endregion

        private GeometryGroup _geometry;

        public ScoreThumbnailView()
        {
            InitializeComponent();
        }

        private void UpdateViewbox()
        {
            Rect bounds = _geometry.Bounds;
            int viewBoxTop = (int)bounds.Top;
            int viewBoxHeight = (int)bounds.Height;

            if (viewBoxHeight < 10)
            {
                viewBoxTop -= 5 - (viewBoxHeight >> 1);
                viewBoxHeight = 10;
            }

            contentBrush.Viewbox = new Rect(0, viewBoxTop, Score.Pattern.Length, viewBoxHeight);
        }

        #region Callbacks

        private void OnGeometryChanged(object sender, EventArgs e) => UpdateViewbox();

        private void OnPatternPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pattern.Length))
            {
                UpdateViewbox();
            }
        }

        private void OnScorePropertyChanged(Score oldScore, Score newScore)
        {
            if (oldScore != null)
            {
                oldScore.Pattern.PropertyChanged -= OnPatternPropertyChanged;
            }

            if (newScore != null)
            {
                newScore.Pattern.PropertyChanged += OnPatternPropertyChanged;
            }

            _geometry = ThumbnailManager.Instance.GetThumbnailForScore(newScore);
            _geometry.Changed += OnGeometryChanged;
            thumbnailPath.Data = _geometry;

            UpdateViewbox();
        }

        private static void ScorePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ScoreThumbnailView)?.OnScorePropertyChanged(e.OldValue as Score, e.NewValue as Score);
        }

        #endregion
    }
}
