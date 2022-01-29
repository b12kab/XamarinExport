using System;
using System.IO;

namespace Export.Helper
{
    public interface IFile
    {
        bool IsFilePermissionNeeded();
        ////bool CreateOutputStream(string filename, out string androidUri, out StreamWriter stream);
        bool CreateOutputStream(string filename, out string androidUri, out Stream stream);
    }
}
