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
            Style defaultKeyStyle = FindResource("PianoRollKeyStyle") as Style;
            Style blackKeyStyle = FindResource("PianoRollBlackKeyStyle") as Style;
            Style keyStyle;
            string[] keyName = new string[]
            {
                "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
            };

            for (int i = 0; i < 128; i++)
            {
                int keyNum = 127 - i;
                int octave = keyNum / 12 - 1;
                int n = keyNum % 12;

                switch (n)
                {
                    case 1:
                    case 3:
                    case 6:
                    case 8:
                    case 10:
                        keyStyle = blackKeyStyle;
                        break;
                    default:
                        keyStyle = defaultKeyStyle;
                        break;
                }

                Button btn = new Button
                {
                    Content = keyName[n] + octave,
                    Style = keyStyle
                };
                buttonContainerElement.Children.Add(btn);
            }
        }
    }
}
