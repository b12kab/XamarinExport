using System;
using System.IO;
using Android.App;
using Android.Database;
using Export.Helper;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(Export.Droid.Implementation.GetFilespec))]
namespace Export.Droid.Implementation
{
    public class GetFilespec : IFile
    {
        /// <summary>
        /// Get device directory to set the file into
        /// </summary>
        /// <returns>device specific directory</returns>
        public string GetDirectory()
        {
            return Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
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
            bool permissionGranted = false;
            const string mimeUriFile = "file://";
            const string excelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            Version version = DeviceInfo.Version;

            // This should not be used at or after version 8 (API 26)
            if (version.Major >= 8)
                return true;

            try
            {
                var context = Android.App.Application.Context;
                permissionGranted = true;
                string fileSpec = Path.Combine(fileDir, fileName);
                FileInfo fileInfo = new FileInfo(fileSpec);
                if (fileInfo.Exists)
                {
                    // Notify the user about the completed "download"
                    DownloadManager downloadManager = DownloadManager.FromContext(context);

                    // https://www.sandersdenardi.com/using-the-android-downloadmanager/
                    // https://docs.microsoft.com/en-us/dotnet/api/android.app.downloadmanager.invokequery?view=xamarin-android-sdk-9#Android_App_DownloadManager_InvokeQuery_Android_App_DownloadManager_Query_
                    // have we already added the file to the download manager?
                    DownloadManager.Query query = new DownloadManager.Query();
                    query.SetFilterByStatus(DownloadStatus.Pending | // <- note bitwise OR
                                            DownloadStatus.Running |
                                            DownloadStatus.Successful);

                    ICursor cursor = downloadManager.InvokeQuery(query);

                    int colDescrption = cursor.GetColumnIndex(DownloadManager.ColumnDescription);
                    int colDirectory = cursor.GetColumnIndex(DownloadManager.ColumnLocalUri);
                    int colMimeType = cursor.GetColumnIndex(DownloadManager.ColumnMediaType);
                    int colFileName = cursor.GetColumnIndex(DownloadManager.ColumnTitle);

                    bool exactMatchFile = false;
                    bool exactMatchDir = false;
                    bool exactMatchDesc = false;

                    for (cursor.MoveToFirst(); !cursor.IsAfterLast; cursor.MoveToNext())
                    {
                        string dmFileName = cursor.GetString(colFileName);
                        if (string.IsNullOrEmpty(dmFileName))
                            dmFileName = string.Empty;

                        if (dmFileName.Equals(fileName))
                            exactMatchFile = true;

                        string dmDesc = cursor.GetString(colDescrption);
                        if (string.IsNullOrEmpty(dmDesc))
                            dmDesc = string.Empty;

                        if (dmDesc.Equals(fileDescription))
                            exactMatchDesc = true;

                        string dmMimeType = cursor.GetString(colMimeType);
                        if (string.IsNullOrEmpty(dmMimeType))
                            dmMimeType = string.Empty;

                        string dmDirectory = cursor.GetString(colDirectory);
                        if (string.IsNullOrEmpty(dmDirectory))
                        {
                            dmDirectory = string.Empty;
                        }
                        else
                        {
                            int i = dmDirectory.IndexOf(mimeUriFile, StringComparison.Ordinal);
                            if (i >= 0)
                            {
                                dmDirectory = dmDirectory.Substring(mimeUriFile.Length);
                            }
                        }

                        if (dmDirectory.Equals(fileDir))
                            exactMatchDir = true;

                        if (exactMatchFile && exactMatchDir && exactMatchDesc)
                            break;
                    }

                    cursor.Close();
                    query = null;

                    if (!(exactMatchFile && exactMatchDir && exactMatchDesc))
                    {
                        downloadManager.AddCompletedDownload(fileInfo.Name, fileDescription, true, excelMimeType,
                                                             fileInfo.DirectoryName, fileInfo.Length, true);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SetDownload() filespec: " + fileDir +
                    " desc: " + fileDescription + " permission granted " + permissionGranted +
                    " - exception: " + ex.Message);
                System.Diagnostics.Debug.Flush();
            }

            return false;
        }
    }
}
