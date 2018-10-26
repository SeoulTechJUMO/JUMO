using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JUMO.UI.Views
{
    public partial class PlaybackTimeView : UserControl
    {
        public static readonly DependencyProperty SecondaryForegroundProperty =
            DependencyProperty.Register(
                "SecondaryForeground", typeof(Brush), typeof(PlaybackTimeView),
                new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public Brush SecondaryForeground
        {
            get => (Brush)GetValue(SecondaryForegroundProperty);
            set => SetValue(SecondaryForegroundProperty, value);
        }

        public PlaybackTimeView()
        {
            InitializeComponent();
        }
    }
}
