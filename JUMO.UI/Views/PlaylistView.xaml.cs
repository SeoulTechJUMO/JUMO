using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using JUMO.UI.Controls;

namespace JUMO.UI.Views
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView : UserControl
    {
        public PlaylistView()
        {
            InitializeComponent();
        }

        private void MusicalCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                PlaylistViewModel vm = (PlaylistViewModel)DataContext;
                RelayCommand zoomCmd = e.Delta > 0 ? vm.ZoomInCommand : vm.ZoomOutCommand;

                if (zoomCmd.CanExecute(null))
                {
                    zoomCmd.Execute(null);
                }

                e.Handled = true;
            }
        }

        private void PlaylistCanvas_PlacePatternRequested(object sender, PlacePatternRequestedEventArgs e)
        {
            PlaylistViewModel vm = (PlaylistViewModel)DataContext;

            vm.PlacePattern(e.Pattern, e.TrackIndex, e.Start);
        }

        private void PlaylistCanvas_RemovePatternRequested(object sender, RemovePatternRequestedEventArgs e)
        {
            PlaylistViewModel vm = (PlaylistViewModel)DataContext;

            vm.RemovePattern(e.PatternToRemove);
        }

        private void PlaylistCanvas_SelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PlaylistViewModel vm = (PlaylistViewModel)DataContext;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    vm.SelectItems(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    vm.DeselectItems(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    vm.ClearSelection();
                    break;
            }
        }
    }
}
