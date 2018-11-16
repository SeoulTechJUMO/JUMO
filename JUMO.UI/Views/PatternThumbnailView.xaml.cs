using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JUMO.UI.Views
{
    public partial class PatternThumbnailView : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty PatternProperty =
            DependencyProperty.Register(
                nameof(Pattern), typeof(Pattern), typeof(PatternThumbnailView),
                new FrameworkPropertyMetadata(PatternPropertyChangedCallback)
            );

        public static readonly DependencyProperty CropLengthProperty =
            DependencyProperty.Register(
                nameof(CropLength), typeof(int), typeof(PatternThumbnailView),
                new FrameworkPropertyMetadata(-1, CropLengthPropertyChangedCallback)
            );

        #endregion

        #region Properties

        public Pattern Pattern
        {
            get => (Pattern)GetValue(PatternProperty);
            set => SetValue(PatternProperty, value);
        }

        public int CropLength
        {
            get => (int)GetValue(CropLengthProperty);
            set => SetValue(CropLengthProperty, value);
        }

        #endregion

        private Geometry _geometry;

        public PatternThumbnailView()
        {
            InitializeComponent();
        }

        private void UpdateViewbox()
        {
            Rect bounds = _geometry?.Bounds ?? new Rect(0, 0, 0, 0);
            int viewBoxTop = (int)bounds.Top;
            int viewBoxHeight = (int)bounds.Height;

            if (viewBoxHeight < 10)
            {
                viewBoxTop -= 5 - (viewBoxHeight >> 1);
                viewBoxHeight = 10;
            }

            contentBrush.Viewbox = new Rect(0, viewBoxTop, Pattern?.Length ?? 0, viewBoxHeight);
        }

        #region Callbacks

        private void OnPatternPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pattern.Length))
            {
                UpdateViewbox();
            }
        }

        private void OnGeometryChanged(object sender, EventArgs e) => UpdateViewbox();

        private void OnPatternPropertyChanged(Pattern oldPattern, Pattern newPattern)
        {
            if (oldPattern != null)
            {
                oldPattern.PropertyChanged -= OnPatternPropertyChanged;
            }

            if (newPattern != null)
            {
                _geometry = ThumbnailManager.Instance.GetThumbnailForPattern(newPattern);
                _geometry.Changed += OnGeometryChanged;
                newPattern.PropertyChanged += OnPatternPropertyChanged;
                thumbnailPath.Data = _geometry;

                UpdateViewbox();
            }
        }

        private static void PatternPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PatternThumbnailView)d).OnPatternPropertyChanged(e.OldValue as Pattern, e.NewValue as Pattern);
        }

        private static void CropLengthPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.OldValue != (int)e.NewValue)
            {
                ((PatternThumbnailView)d).UpdateViewbox();
            }
        }

        #endregion
    }
}
