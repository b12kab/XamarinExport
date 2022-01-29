using System;
using System.IO;
using Export.Helper;

[assembly: Xamarin.Forms.Dependency(typeof(Export.iOS.Implementation.GetFilespec))]
namespace Export.iOS.Implementation
{
    public class GetFilespec : IFile
    {
        /// <summary>
        /// iOS will always return true for file write, as all files are local
        /// </summary>
        /// <returns>Permission needed? always true</returns>
        public bool IsFilePermissionNeeded()
        {
            return false;
        }

        /// <summary>
        /// Write file out to app directory.
        /// </summary>
        /// <param name="filename">Filename to export content into</param>
        /// <param name="androidUri">Not used on iOS</param>
        /// <param name="stream">StreamWriter</param>
        /// <returns>true = worked w/o error or false = failed with system error</returns>
        public bool CreateOutputStream(string filename, out string androidUri, out Stream stream)
        {
            androidUri = null;
            stream = null;

            string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Documents folder  

            string filespec = Path.Combine(dir, filename);

            FileInfo newFile = new FileInfo(filespec);
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();

                    System.Diagnostics.Debug.WriteLine("existing file deleted: " + filespec);
                    System.Diagnostics.Debug.Flush();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed file delete. Passed in filename: " + filespec + ". Full filespec: " + filespec);
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                    System.Diagnostics.Debug.Flush();
                    return false;
                }
            }

            try
            {
                ////stream = new StreamWriter(filespec);
                stream = new FileStream(filespec, FileMode.OpenOrCreate);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed file write. Passed in filename: " + filespec + ". Full filespec: " + filespec);
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                System.Diagnostics.Debug.Flush();
                return false;
            }

            return true;
        }

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
