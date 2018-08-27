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

        #region Routed Events

        public static readonly RoutedEvent KeyPressedEvent =
            EventManager.RegisterRoutedEvent(
                "KeyPressed",
                RoutingStrategy.Direct,
                typeof(PianoRollKeyEventHandler),
                typeof(PianoRollKeyboard)
            );

        public static readonly RoutedEvent KeyReleasedEvent =
            EventManager.RegisterRoutedEvent(
                "KeyReleased",
                RoutingStrategy.Direct,
                typeof(PianoRollKeyEventHandler),
                typeof(PianoRollKeyboard)
            );

        public event PianoRollKeyEventHandler KeyPressed
        {
            add => AddHandler(KeyPressedEvent, value);
            remove => RemoveHandler(KeyPressedEvent, value);
        }

        public event PianoRollKeyEventHandler KeyReleased
        {
            add => AddHandler(KeyReleasedEvent, value);
            remove => RemoveHandler(KeyReleasedEvent, value);
        }

        #endregion

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
                    Style = keyStyle,
                    Tag = keyNum
                };
                btn.PreviewMouseLeftButtonDown += Btn_PreviewMouseLeftButtonDown;
                btn.PreviewMouseLeftButtonUp += Btn_PreviewMouseLeftButtonUp;
                buttonContainerElement.Children.Add(btn);
            }
        }

        private void Btn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            Point pos = e.GetPosition(btn);
            RaiseEvent(new PianoRollKeyEventArgs(KeyPressedEvent, Convert.ToByte(btn.Tag), (byte)(pos.X * 127 / btn.ActualWidth)));
        }

        private void Btn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            RaiseEvent(new PianoRollKeyEventArgs(KeyReleasedEvent, Convert.ToByte(btn.Tag), 127));
        }
    }

    public delegate void PianoRollKeyEventHandler(object sender, PianoRollKeyEventArgs e);

    public class PianoRollKeyEventArgs : RoutedEventArgs
    {
        public byte NoteValue { get; }
        public byte Velocity { get; }

        public PianoRollKeyEventArgs(RoutedEvent id, byte value, byte velocity)
        {
            RoutedEvent = id;
            NoteValue = value;
            Velocity = velocity;
        }
    }
}
