using JUMO.UI.Services;

namespace JUMO.UI
{
    class FileDialogViewModel : ViewModelBase
    {
        public override string DisplayName => "";

        public string Title { get; set; }
        public string Extension { get; set; }
        public string Filter { get; set; }
        public string FileName { get; set; }

        public RelayCommand ShowOpenCommand { get; }
        public RelayCommand ShowSaveCommand { get; }

        public FileDialogViewModel()
        {
            ShowOpenCommand = new RelayCommand(ExecuteShowOpen);
            ShowSaveCommand = new RelayCommand(ExecuteShowSave);
        }

        private void ExecuteShowOpen(object _)
        {
            FileName = FileDialogService.ShowOpenFileDialog(Title, Extension, Filter);
        }

        private void ExecuteShowSave(object _)
        {
            FileName = FileDialogService.ShowSaveFileDialog(Title, Extension, Filter);
        }
    }
}
