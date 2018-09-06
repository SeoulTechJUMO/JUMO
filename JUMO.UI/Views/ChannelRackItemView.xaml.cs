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
    /// <summary>
    /// Interaction logic for ChannelRackItemView.xaml
    /// </summary>
    public partial class ChannelRackItemView : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty PluginProperty =
            DependencyProperty.Register(
                "Plugin", typeof(Vst.Plugin), typeof(ChannelRackItemView)
            );

        public static readonly DependencyProperty ScoreProperty =
            DependencyProperty.Register(
                "Score", typeof(Score), typeof(ChannelRackItemView)
            );

        public static readonly DependencyProperty OpenPluginEditorProperty =
            DependencyProperty.Register(
                "OpenPluginEditor", typeof(ICommand), typeof(ChannelRackItemView)
            );

        public static readonly DependencyProperty OpenPianoRollProperty =
            DependencyProperty.Register(
                "OpenPianoRoll", typeof(ICommand), typeof(ChannelRackItemView)
            );

        #endregion

        #region Properties

        public Vst.Plugin Plugin
        {
            get => (Vst.Plugin)GetValue(PluginProperty);
            set => SetValue(PluginProperty, value);
        }

        public Score Score
        {
            get => (Score)GetValue(ScoreProperty);
            set => SetValue(ScoreProperty, value);
        }

        public ICommand OpenPluginEditor
        {
            get => (ICommand)GetValue(OpenPluginEditorProperty);
            set => SetValue(OpenPluginEditorProperty, value);
        }

        public ICommand OpenPianoRoll
        {
            get => (ICommand)GetValue(OpenPianoRollProperty);
            set => SetValue(OpenPianoRollProperty, value);
        }

        #endregion

        public ChannelRackItemView()
        {
            InitializeComponent();

            root.DataContext = this;
        }
    }
}
