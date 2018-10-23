using JUMO.UI.Views;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JUMO.UI.Controls
{
    public class VolumeMeter : FrameworkElement
    {
        public VolumeMeter()
        {
            _DeciColor.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x76, 0xFF, 0x42), 1));
            _DeciColor.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xF0, 0x1D), 0.2));
            _DeciColor.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFF, 0x3D, 0x3D), 0));

            MinDb = -60;
            MaxDb = 18;
            Amplitude = 0;
        }

        #region Dependency Properties

        public static readonly DependencyProperty AmplitudeProperty =
            DependencyProperty.Register("Amplitude",
                typeof(float),
                typeof(VolumeMeter),
                new FrameworkPropertyMetadata(0f, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        #endregion

        #region DrawingProperty

        private Rect BackgroundRect;
        private Rect DeciRect;
        private Rect Block;


        private SolidColorBrush _BackgroundColor = new SolidColorBrush()
        {
            Color = Colors.Gray
        };

        private SolidColorBrush _BorderColor = new SolidColorBrush()
        {
            Color = Colors.LightGray
        };

        private LinearGradientBrush _DeciColor = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1),
        };

        #endregion

        #region VolumeProperty

        /// <summary>
        /// Current Value
        /// </summary>
        [DefaultValue(-60.0)]
        public float Amplitude
        {
            get => (float)GetValue(AmplitudeProperty);
            set { SetValue(AmplitudeProperty, value); }
        }

        /// <summary>
        /// Minimum decibels
        /// </summary>
        [DefaultValue(-60.0)]
        public float MinDb { get; set; }

        /// <summary>
        /// Maximum decibels
        /// </summary>
        [DefaultValue(18.0)]
        public float MaxDb { get; set; }

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            double rw = RenderSize.Width;
            double rh = RenderSize.Height;

            double db = 20 * Math.Log10(Amplitude);
            if (db < MinDb)
                db = MinDb;
            if (db > MaxDb)
                db = MaxDb;
            double percent = (db - MinDb) / (MaxDb - MinDb);
 
            BackgroundRect.Width = rw;
            BackgroundRect.Height = rh;

            DeciRect.Width = rw;
            DeciRect.Height = rh;

            dc.DrawRectangle(_BackgroundColor, null, BackgroundRect);
            dc.DrawRectangle(_DeciColor, null, DeciRect);

            Block.Width = rw;
            Block.Height = rh - rh * percent;

            dc.DrawRectangle(_BackgroundColor, null, Block);
        }
    }
}
