using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.File
{
    public class ProjectReader
    {
        private const uint Version = 0;

        private static readonly byte[] MagicBytes = new byte[4] { 75, 73, 65, 126 };

        public void LoadFile(string path)
        {
            try
            {
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    bool magicOK = CheckMagic(stream);
                    bool versionOK = CheckVersion(stream);

                    if (magicOK && versionOK)
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        ProjectFile file = (ProjectFile)formatter.Deserialize(stream);
                    }
                    else
                    {
                        // Fail
                    }
                }
            }
            catch
            {
                // Fail
            }
        }

        private bool CheckMagic(Stream stream)
        {
            byte[] buf = new byte[4];

            if (stream.Read(buf, 0, 4) == 4)
            {
                return buf[0] == MagicBytes[0] && buf[1] == MagicBytes[1] && buf[2] == MagicBytes[2] && buf[3] == MagicBytes[3];
            }

            return false;
        }

        private bool CheckVersion(Stream stream)
        {
            byte[] buf = new byte[4];

            if (stream.Read(buf, 0, 4) == 4)
            {
                uint parsedVersion = BitConverter.ToUInt32(buf, 0);

                return parsedVersion == Version;
            }

            return false;
        }
    }
}
