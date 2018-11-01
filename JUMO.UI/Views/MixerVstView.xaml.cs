using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JUMO.UI.Views
{
    public partial class MixerVstView : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty PluginProperty =
            DependencyProperty.Register(nameof(Plugin), typeof(Vst.EffectPlugin), typeof(MixerVstView));

        public static readonly DependencyProperty OpenPluginEditorProperty =
            DependencyProperty.Register(nameof(OpenPluginEditor), typeof(ICommand), typeof(MixerVstView));

        public static readonly DependencyProperty ReplacePluginProperty =
             DependencyProperty.Register(nameof(ReplacePlugin), typeof(ICommand), typeof(MixerVstView));

        public static readonly DependencyProperty RemovePluginProperty =
            DependencyProperty.Register(nameof(RemovePlugin), typeof(ICommand), typeof(MixerVstView));

        #endregion

        #region Properties

        public Vst.EffectPlugin Plugin
        {
            get => (Vst.EffectPlugin)GetValue(PluginProperty);
            set => SetValue(PluginProperty, value);
        }

        public ICommand OpenPluginEditor
        {
            get => (ICommand)GetValue(OpenPluginEditorProperty);
            set => SetValue(OpenPluginEditorProperty, value);
        }

        public ICommand ReplacePlugin
        {
            get => (ICommand)GetValue(ReplacePluginProperty);
            set => SetValue(ReplacePluginProperty, value);
        }

        public ICommand RemovePlugin
        {
            get => (ICommand)GetValue(RemovePluginProperty);
            set => SetValue(RemovePluginProperty, value);
        }

        #endregion

        public MixerVstView()
        {
            InitializeComponent();

            root.DataContext = this;
        }
    }
}
