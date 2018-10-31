using Microsoft.Win32;

namespace JUMO.UI
{
    class FileDialogService
    {
        public string ShowOpenFileDialog(string title, string extension, string filter)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Title = title,
                DefaultExt = extension,
                Filter = filter
            };

            if (dlg.ShowDialog() ?? false)
            {
                return dlg.FileName;
            }

            return null;
        }

        public string ShowSaveFileDialog(string title, string extension, string filter)
        {
            SaveFileDialog dlg = new SaveFileDialog()
            {
                Title = title,
                DefaultExt = extension,
                Filter = filter
            };

            if (dlg.ShowDialog() ?? false)
            {
                return dlg.FileName;
            }

            return null;
        }
    }
}
