using System;
namespace Export.Helper
{
    public interface IFile
    {
        string GetDirectory();
        string GenerateFilespec(string directory, string filename);
        bool SetDownload(string fileDir, string fileName, string fileDescription);
    }
}
