﻿using System;
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
    /// <summary>
    /// VolumeMeterView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VolumeMeterView : UserControl
    {
        #region Dependency Properties

        //public static readonly DependencyProperty PluginProperty =
        //    DependencyProperty.Register(
        //        "Plugin", typeof(Vst.Plugin), typeof(ChannelRackItemView)
        //    );
        #endregion

        #region Properties

        //public double LeftVolume
        //{
        //    get => (double)GetValue(PluginProperty);
        //    set => SetValue(PluginProperty, value);
        //}



        #endregion

        public VolumeMeterView()
        {
            InitializeComponent();
        }
    }
}
