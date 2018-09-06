using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;

namespace JUMO.UI.Controls
{
    class ScoreThumbnail : FrameworkElement
    {
        #region Dependency Properties

        public static readonly DependencyProperty ScoreProperty =
            DependencyProperty.Register(
                "Score", typeof(Score), typeof(ScoreThumbnail),
                new FrameworkPropertyMetadata(ScorePropertyChangedCallback)
            );

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                "Background", typeof(Brush), typeof(ScoreThumbnail),
                new FrameworkPropertyMetadata(
                    Brushes.White,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                "Foreground", typeof(Brush), typeof(ScoreThumbnail),
                new FrameworkPropertyMetadata(
                    Brushes.Black,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        #endregion

        #region Properties

        public Score Score
        {
            get => (Score)GetValue(ScoreProperty);
            set => SetValue(ScoreProperty, value);
        }

        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(RenderSize));

            if ((Score?.Count ?? 0) == 0)
            {
                return;
            }
        }

        private void OnScoreChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void OnScorePropertyChanged(Score oldScore, Score newScore)
        {
            if (oldScore != null)
            {
                oldScore.CollectionChanged -= OnScoreChanged;
            }

            if (newScore != null)
            {
                newScore.CollectionChanged += OnScoreChanged;
            }

            InvalidateVisual();
        }

        private static void ScorePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ScoreThumbnail)?.OnScorePropertyChanged(e.OldValue as Score, e.NewValue as Score);
        }
    }
}
