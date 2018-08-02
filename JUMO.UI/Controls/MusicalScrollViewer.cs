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

namespace JUMO.UI.Controls
{
    public class MusicalScrollViewer : ContentControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register(
                "HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(
                    ScrollBarVisibility.Visible,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                )
            );

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register(
                "VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(
                    ScrollBarVisibility.Visible,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                )
            );

        public static readonly DependencyProperty ShouldDrawHorizontalGridProperty =
            DependencyProperty.Register(
                "ShouldDrawHorizontalGrid", typeof(bool), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.AffectsRender
                )
            );

        public static readonly DependencyProperty GridHeightProperty =
            DependencyProperty.Register(
                "GridHeight", typeof(double), typeof(MusicalScrollViewer),
                new FrameworkPropertyMetadata(
                    20.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                )
            );

        #endregion

        #region Dependency Property Accessors

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            set => SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            set => SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        public bool ShouldDrawHorizontalGrid
        {
            get => (bool)GetValue(ShouldDrawHorizontalGridProperty);
            set => SetValue(ShouldDrawHorizontalGridProperty, value);
        }

        public double GridHeight
        {
            get => (double)GetValue(GridHeightProperty);
            set => SetValue(GridHeightProperty, value);
        }

        #endregion

        static MusicalScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MusicalScrollViewer), new FrameworkPropertyMetadata(typeof(MusicalScrollViewer)));
        }
    }
}
