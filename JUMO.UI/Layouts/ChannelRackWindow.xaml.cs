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
using System.Windows.Shapes;

namespace JUMO.UI.Layouts
{
    public partial class ChannelRackWindow : Window
    {
        public ChannelRackWindow()
        {
            InitializeComponent();
        }

        private void CanExecuteOpenPluginEditor(object sender, CanExecuteRoutedEventArgs e)
        {
            // TODO: VST 플러그인이 에디터 UI를 제공하는지 확인해야 함. (Flag, CanDo 등을 조사)
            e.CanExecute = true;
        }

        private void OpenPluginEditorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            new PluginEditorWindow(e.Parameter as Vst.Plugin).Show();
        }
    }
}
