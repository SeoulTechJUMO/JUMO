using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JUMO.UI.Data;
using JUMO.UI.Views;

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

    abstract class VirtualNoteBase : IVirtualElement
    {
        protected readonly Note _note;
        protected Segment _bounds;

        public Segment Bounds => _bounds;
        public UIElement Visual { get; protected set; }

        public event EventHandler BoundsChanged;

        public VirtualNoteBase(Note note)
        {
            _note = note;
            _bounds = new Segment(_note.Start, _note.Length);
            _note.PropertyChanged += OnNotePropertyChanged;
        }

        public abstract UIElement CreateVisual(MusicalCanvasBase parent);

        public void DisposeVisual()
        {
            Visual = null;
        }

        private void OnNotePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Note note = (Note)sender;
            _bounds.Start = _note.Start;
            _bounds.Length = _note.Length;

            BoundsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    class VirtualNote : VirtualNoteBase
    {
        public VirtualNote(Note note) : base(note) { }

        public override UIElement CreateVisual(MusicalCanvasBase parent)
        {
            if (Visual == null)
            {
                Visual = new NoteView() { DataContext = _note };
            }

            return Visual;
        }
    }

    class VirtualVelocityControl : VirtualNoteBase
    {
        public VirtualVelocityControl(Note note) : base(note) { }

        public override UIElement CreateVisual(MusicalCanvasBase parent)
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
    }
}