using System;
using System.Collections.Specialized;
using System.ComponentModel;
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

            if (Score == null || Score.Count == 0 || Score.Pattern.Length == 0)
            {
                return;
            }

            (Note minValueNote, Note maxValueNote) = Data.EnumHelper.MinMaxBy(Score, note => note.Value);
            byte minValue = minValueNote.Value;
            byte maxValue = maxValueNote.Value;
            int valueRangeSize = maxValue - minValue + 1;

            double tickWidth = RenderSize.Width / Score.Pattern.Length;
            double noteHeight = RenderSize.Height / (valueRangeSize > 10 ? valueRangeSize : 10);
            double top = valueRangeSize < 10 ? (10 - valueRangeSize) * noteHeight / 2 : 0;

            foreach (Note note in Score)
            {
                Point point = new Point(note.Start * tickWidth, top + (maxValue - note.Value) * noteHeight);
                Size size = new Size(note.Length * tickWidth, noteHeight);

                dc.DrawRectangle(Foreground, null, new Rect(point, size));
            }
        }

        #region Callbacks

        private void OnNotePropertyChanged(object sender, EventArgs e) => InvalidateVisual();

        private void OnScoreChanged(object sender, NotifyCollectionChangedEventArgs e) => InvalidateVisual();

        private void OnPatternPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pattern.Length))
            {
                InvalidateVisual();
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

            InvalidateVisual();
        }

        private static void ScorePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ScoreThumbnail)?.OnScorePropertyChanged(e.OldValue as Score, e.NewValue as Score);
        }

        #endregion
    }
}
