using System;
using System.IO;
using Export.Helper;

[assembly: Xamarin.Forms.Dependency(typeof(Export.iOS.Implementation.GetFilespec))]
namespace Export.iOS.Implementation
{
    public class GetFilespec : IFile
    {
        /// <summary>
        /// Get device directory to set the file into
        /// </summary>
        /// <returns>device specific directory</returns>
        public string GetDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Documents folder  
        }

        /// <summary>
        /// Generate a full filespec from the directory and filename
        /// </summary>
        /// <param name="directory">device specific directory</param>
        /// <param name="filename">export filename</param>
        /// <returns>Full filespec</returns>
        public string GenerateFilespec(string directory, string filename)
        {
            string filespec = Path.Combine(directory, filename);

            FileInfo newFile = new FileInfo(filespec);
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook

                    System.Diagnostics.Debug.WriteLine("existing file deleted: " + filespec);
                    System.Diagnostics.Debug.Flush();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("existing file failed delete: " + filespec);
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                    System.Diagnostics.Debug.Flush();
                }
            }

            return filespec;
        }

        /// <summary>
        /// This will not be used in iOS
        /// </summary>
        /// <param name="fileDir">File directory</param>
        /// <param name="fileName">File name</param>
        /// <param name="fileDescription">Description to be used to add to the download manager</param>
        /// <returns>status - true: worked; false: failure</returns>
        public bool SetDownload(string fileDir, string fileName, string fileDescription)
        {
            return true;
        }

    }
}
