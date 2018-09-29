using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JUMO.UI.Views
{
    public partial class PatternThumbnailView : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty PatternProperty =
            DependencyProperty.Register(
                "Pattern", typeof(Pattern), typeof(PatternThumbnailView),
                new FrameworkPropertyMetadata(PatternPropertyChangedCallback)
            );

        #endregion

        #region Properties

        public Pattern Pattern
        {
            get => (Pattern)GetValue(PatternProperty);
            set => SetValue(PatternProperty, value);
        }

        #endregion

        private readonly GeometryGroup _geometry = new GeometryGroup() { FillRule = FillRule.Nonzero };

        public PatternThumbnailView()
        {
            InitializeComponent();
        }

        #region Callbacks

        private void OnPatternPropertyChanged(Pattern oldPattern, Pattern newPattern)
        {
            GeometryCollection gc = _geometry.Children;
            Rect bounds = Rect.Empty;

            gc.Clear();

            foreach (Vst.Plugin plugin in Vst.PluginManager.Instance.Plugins)
            {
                GeometryGroup geometry = ThumbnailManager.Instance[Pattern[plugin]];
                bounds.Union(geometry.Bounds);

                gc.Add(geometry);
            }

            bounds.Width = Pattern.Length;
            contentBrush.Viewbox = bounds;
            thumbnailPath.Data = _geometry;
        }

        private static void PatternPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PatternThumbnailView)d).OnPatternPropertyChanged(e.OldValue as Pattern, e.NewValue as Pattern);
        }

        #endregion
    }
}
