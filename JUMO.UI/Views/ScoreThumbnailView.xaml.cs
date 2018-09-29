using System;
using System.Collections.Specialized;
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

        private readonly GeometryGroup _geometry = new GeometryGroup() { FillRule = FillRule.Nonzero };

        public ScoreThumbnailView()
        {
            InitializeComponent();

            thumbnailPath.Data = _geometry;
        }

        private void RefreshGeometry()
        {
            GeometryCollection gc = _geometry.Children;

            gc.Clear();

            if (Score == null || Score.Count == 0 || Score.Pattern.Length == 0)
            {
                return;
            }

            int maxValue = 0;
            int minValue = 127;

            foreach (Note note in Score)
            {
                maxValue = maxValue < note.Value ? note.Value : maxValue;
                minValue = minValue > note.Value ? note.Value : minValue;

                gc.Add(new RectangleGeometry(new Rect(note.Start, 127 - note.Value, note.Length, 1)));
            }

            int viewBoxTop = 127 - maxValue;
            int viewBoxHeight = maxValue - minValue + 1;

            if (viewBoxHeight < 10)
            {
                viewBoxTop -= 5 - (viewBoxHeight >> 1);
                viewBoxHeight = 10;
            }

            contentBrush.Viewbox = new Rect(0, viewBoxTop, Score.Pattern.Length, viewBoxHeight);
        }

        #region Callbacks

        private void OnNotePropertyChanged(object sender, EventArgs e) => RefreshGeometry();

        private void OnScoreChanged(object sender, NotifyCollectionChangedEventArgs e) => RefreshGeometry();

        private void OnPatternPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pattern.Length))
            {
                RefreshGeometry();
            }
        }

        private void OnScorePropertyChanged(Score oldScore, Score newScore)
        {
            if (oldScore != null)
            {
                oldScore.CollectionChanged -= OnScoreChanged;
                oldScore.NotePropertyChanged -= OnNotePropertyChanged;
                oldScore.Pattern.PropertyChanged -= OnPatternPropertyChanged;
            }

            if (newScore != null)
            {
                newScore.CollectionChanged += OnScoreChanged;
                newScore.NotePropertyChanged += OnNotePropertyChanged;
                newScore.Pattern.PropertyChanged += OnPatternPropertyChanged;
            }

            RefreshGeometry();
        }

        private static void ScorePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ScoreThumbnailView)?.OnScorePropertyChanged(e.OldValue as Score, e.NewValue as Score);
        }

        #endregion
    }
}
