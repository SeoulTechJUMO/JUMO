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
        bool IsSelected { get; set; }
        UIElement CreateVisual(MusicalCanvasBase parent);
        void DisposeVisual();
        event EventHandler BoundsChanged;
    }

    abstract class VirtualElement : IVirtualElement
    {
        private bool _isSelected = false;

        public abstract event EventHandler BoundsChanged;

        public abstract Segment Bounds { get; }
        public UIElement Visual { get; protected set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnIsSelectedChanged();
                }
            }
        }

        public abstract UIElement CreateVisual(MusicalCanvasBase parent);

        public virtual void DisposeVisual() => Visual = null;

        protected virtual void OnIsSelectedChanged() { }
    }

    abstract class VirtualNoteBase : VirtualElement
    {
        protected readonly Note _note;
        protected Segment _bounds;

        public override event EventHandler BoundsChanged;

        public override Segment Bounds => _bounds;

        public VirtualNoteBase(Note note)
        {
            _note = note;
            _bounds = new Segment(_note.Start, _note.Length);
            _note.PropertyChanged += OnNotePropertyChanged;
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
                Visual = new NoteView()
                {
                    DataContext = _note,
                    IsSelected = IsSelected
                };
            }

            return Visual;
        }

        protected override void OnIsSelectedChanged()
        {
            if (Visual != null)
            {
                ((NoteView)Visual).IsSelected = IsSelected;
            }
        }
    }

    class VirtualVelocityControl : VirtualNoteBase
    {
        public VirtualVelocityControl(Note note) : base(note) { }

        public override UIElement CreateVisual(MusicalCanvasBase parent)
        {
            if (Visual == null)
            {
                Visual = new NoteVelocityView()
                {
                    DataContext = _note,
                    IsSelected = IsSelected
                };
            }

            return Visual;
        }

        protected override void OnIsSelectedChanged()
        {
            if (Visual != null)
            {
                ((NoteVelocityView)Visual).IsSelected = IsSelected;
            }
        }
    }

    class VirtualPatternControl : IVirtualElement
    {
        private bool _isSelected = false;
        private Segment _bounds;

        public Segment Bounds => _bounds;

        public UIElement Visual { get; private set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnIsSelectedChanged();
                }
            }
        }

        public event EventHandler BoundsChanged;

        public UIElement CreateVisual(MusicalCanvasBase parent)
        {
            if (Visual == null)
            {
                Visual = new Button() { };
            }

            return Visual;
        }

        public void DisposeVisual()
        {
            Visual = null;
        }

        private void OnIsSelectedChanged()
        {
            //throw new NotImplementedException();
        }
    }
}
