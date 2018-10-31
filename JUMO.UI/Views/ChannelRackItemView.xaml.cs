using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public static readonly DependencyProperty RemovePluginProperty =
            DependencyProperty.Register(
                "RemovePlugin", typeof(ICommand), typeof(ChannelRackItemView)
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

        public ICommand RemovePlugin
        {
            get => (ICommand)GetValue(RemovePluginProperty);
            set => SetValue(RemovePluginProperty, value);
        }

        #endregion

        public ChannelRackItemView()
        {
            InitializeComponent();

            root.DataContext = this;
        }
    }
}
