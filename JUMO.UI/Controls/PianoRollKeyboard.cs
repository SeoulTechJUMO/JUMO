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
    public class PianoRollKeyboard : Control
    {
        private StackPanel buttonContainerElement;

        static PianoRollKeyboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PianoRollKeyboard), new FrameworkPropertyMetadata(typeof(PianoRollKeyboard)));
        }

        public override void OnApplyTemplate()
        {
            buttonContainerElement = GetTemplateChild("ButtonContainer") as StackPanel;
            AddButtons();
        }

        private void AddButtons()
        {
            for (int i = 0; i < 128; i++)
            {
                Button btn = new Button
                {
                    Content = $"Key #{127 - i}"
                };
                buttonContainerElement.Children.Add(btn);
            }

            buttonContainerElement.Children.Add(new System.Windows.Controls.Primitives.ScrollBar()
            {
                Orientation = Orientation.Horizontal,
                Visibility = Visibility.Hidden
            });
        }
    }
}
