using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace JUMO.UI.Controls
{
    class BlockSelectionHelper
    {
        private readonly UIElement _adorned;
        private readonly BlockSelectionAdorner _selectionAdorner;

        public bool IsBlockSelecting { get; set; } = false;

        public BlockSelectionHelper(UIElement adornedElement)
        {
            _adorned = adornedElement;
            _selectionAdorner = new BlockSelectionAdorner(_adorned);
        }

        public void StartBlockSelection(Point startPoint)
        {
            Mouse.Capture(_adorned, CaptureMode.Element);
            AdornerLayer.GetAdornerLayer(_adorned)?.Add(_selectionAdorner);

            _selectionAdorner.Point1 = _selectionAdorner.Point2 = startPoint;
            IsBlockSelecting = true;
        }

        public void UpdateBlockSelection(Point currentPosition)
        {
            _selectionAdorner.Point2 = currentPosition;
        }

        public Rect EndBlockSelection()
        {
            IsBlockSelecting = false;

            AdornerLayer.GetAdornerLayer(_adorned)?.Remove(_selectionAdorner);
            Mouse.Capture(null);

            return new Rect(_selectionAdorner.Point1, _selectionAdorner.Point2);
        }
    }
}
