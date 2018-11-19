using System.Windows;
using System.Windows.Media;

namespace JUMO.UI.Controls
{
    public class PluginEditorHostContainer : FrameworkElement
    {
        private readonly VisualCollection _visualChildren;
        private PluginEditorHost _host;

        #region Dependency Properties

        public static readonly DependencyProperty PluginProperty =
            DependencyProperty.Register(
                nameof(Plugin), typeof(Vst.PluginBase), typeof(PluginEditorHostContainer),
                new FrameworkPropertyMetadata(PluginPropertyChangedCallback)
            );

        #endregion

        #region Properties

        public Vst.PluginBase Plugin
        {
            get => (Vst.PluginBase)GetValue(PluginProperty);
            set => SetValue(PluginProperty, value);
        }

        protected override int VisualChildrenCount => _visualChildren.Count;

        #endregion

        public PluginEditorHostContainer()
        {
            _visualChildren = new VisualCollection(this);
        }

        protected override Visual GetVisualChild(int index) => _visualChildren[index];

        private void OnPluginPropertyChanged(Vst.PluginBase newPlugin)
        {
            if (_host != null)
            {
                _visualChildren.Clear();
                _host.Dispose();
                _host = null;
            }

            if (newPlugin != null)
            {
                _host = new PluginEditorHost(newPlugin);
                _visualChildren.Add(_host);
            }
        }

        private static void PluginPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PluginEditorHostContainer)?.OnPluginPropertyChanged(e.NewValue as Vst.PluginBase);
        }
    }
}
