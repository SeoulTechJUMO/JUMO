using System.Windows;

namespace JUMO.UI
{
    public class BindingProxy : Freezable
    {
        public static DependencyProperty DataProperty =
            DependencyProperty.Register(
                "Data", typeof(object), typeof(BindingProxy),
                new FrameworkPropertyMetadata(null)
            );

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        protected override Freezable CreateInstanceCore() => new BindingProxy();
    }
}
