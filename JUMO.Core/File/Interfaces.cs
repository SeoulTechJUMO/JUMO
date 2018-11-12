using System.IO;

namespace JUMO.File
{
    interface IProjectReader
    {
        void DoLoadFile(Stream stream, string path);
    }

    interface IProjectWriter
    {
        bool DoSaveFile(string path);
    }
}
