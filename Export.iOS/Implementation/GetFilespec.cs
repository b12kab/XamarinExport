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
                stream = new FileStream(filespec, FileMode.OpenOrCreate);
                System.Diagnostics.Debug.WriteLine("Created filespec: " + filespec + ".");
                System.Diagnostics.Debug.Flush();
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
    }
}
