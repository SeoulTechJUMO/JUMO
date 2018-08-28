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
                Visual = new NoteVelocityView() { DataContext = _note };
            }

            return Visual;
        }
    }
}