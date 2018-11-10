﻿using System.Windows;
using JUMO.UI.Controls;

namespace JUMO.UI.Layouts
{
    /// <summary>
    /// SmartMelodyView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmartMelodyWindow : WindowBase
    {
        public SmartMelodyWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SmartMelodyViewModel svm = (SmartMelodyViewModel)DataContext;
            if (!svm.WillInsert) { svm.ChangeScore(svm.CurrentMelody, true); }
        }

        private void InsertButtonClick(object sender, RoutedEventArgs e)
        {
            SmartMelodyViewModel svm = (SmartMelodyViewModel)DataContext;
            svm.WillInsert = true;
            Close();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
