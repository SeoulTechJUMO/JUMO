using JUMO.UI.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JUMO.UI.Controls
{
    interface IVirtualElement
    {
        Segment Bounds { get; }
        UIElement Visual { get; }
        UIElement CreateVisual(MusicalCanvasBase parent);
        void DisposeVisual();
        event EventHandler BoundsChanged;
    }

    class VirtualNote : IVirtualElement
    {
        private readonly INote _note;

        public VirtualNote(INote note)
        {
            _note = note;
            Bounds = new Segment(_note.Start, _note.Length);
        }

        public Segment Bounds { get; }

        public UIElement Visual { get; private set; }

        public event EventHandler BoundsChanged;

        public UIElement CreateVisual(MusicalCanvasBase parent)
        {
            if (Visual == null)
            {
                Button r = new Button()
                {
                    Content = $"{_note.Value}",
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };

                PianoRollCanvas.SetNoteValue(r, _note.Value);
                PianoRollCanvas.SetStart(r, _note.Start);
                PianoRollCanvas.SetLength(r, _note.Length);

                Visual = r;
            }

            return Visual;
        }

        public void DisposeVisual()
        {
            Visual = null;
        }
    }

    class VirtualVelocityControl : IVirtualElement
    {
        private readonly INote _note;

        public VirtualVelocityControl(INote note)
        {
            _note = note;
            Bounds = new Segment(_note.Start, _note.Length);
        }

        public Segment Bounds { get; }

        public UIElement Visual { get; private set; }

        public event EventHandler BoundsChanged;

        public UIElement CreateVisual(MusicalCanvasBase parent)
        {
            if (Visual == null)
            {
                Border b1 = new Border()
                {
                    BorderBrush = Brushes.Fuchsia,
                    BorderThickness = new Thickness(0, 1.5, 0, 0),
                    Background = new SolidColorBrush(Colors.Fuchsia) { Opacity = 0.125 }
                };

                Border b2 = new Border()
                {
                    BorderBrush = Brushes.Fuchsia,
                    BorderThickness = new Thickness(1, 0, 0, 0)
                };

                Ellipse e = new Ellipse()
                {
                    Stroke = Brushes.Fuchsia,
                    StrokeThickness = 1.5,
                    Fill = Brushes.White,
                    Width = 6.0,
                    Height = 6.0,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(-4, -3.5, 0, 0)
                };

                b2.Child = e;
                b1.Child = b2;

                VelocityCanvas.SetVelocity(b1, _note.Velocity);
                VelocityCanvas.SetStart(b1, _note.Start);
                VelocityCanvas.SetLength(b1, _note.Length);

                Visual = b1;
            }

            return Visual;
        }

        public void DisposeVisual()
        {
            Visual = null;
        }
    }
}