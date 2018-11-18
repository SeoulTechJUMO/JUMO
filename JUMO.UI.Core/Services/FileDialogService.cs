using Microsoft.Win32;

namespace JUMO.UI.Services
{
    public static class FileDialogService
    {
        public static string ShowOpenFileDialog(string title, string extension, string filter)
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

        public static string ShowSaveFileDialog(string title, string extension, string filter)
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
