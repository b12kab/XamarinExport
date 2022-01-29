using Android.App;
using Android.Content;
using Android.Database;
using Export.Helper;
using Plugin.CurrentActivity;
using System;
using System.IO;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(Export.Droid.Implementation.GetFilespec))]
namespace Export.Droid.Implementation
{
    public class GetFilespec : IFile
    {
        /// <summary>
        /// After version 10 (API 29) cannot write directly to Downloads directory, so permissions not needed.
        /// Before that (API 28 and earlier), permission is needed.
        /// </summary>
        /// <returns>true, if permission needed, false if not</returns>
        public bool IsFilePermissionNeeded()
        {
            Version version = DeviceInfo.Version;

            if (version.Major < 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Create stream to write file out to
        /// Note: Do not call before IsFilePermissionNeeded
        /// </summary>
        /// <param name="filename">Output stream filename to create</param>
        /// <param name="androidUri">Output Android uri</param>
        /// <param name="stream">Output stream to write to</param>
        /// <returns>true = worked w/o error or false = failed with system error</returns>
        public bool CreateOutputStream(string filename, out string androidUri, out Stream stream)
        {
            Version version = DeviceInfo.Version;

            // After version 10 (API 29) cannot write directly to Downloads directory
            // and must do it via the Mediastore API due to security changes
            if (version.Major < 10)
            {
                androidUri = null;
                return CreateStreamAPI28AndBelow(filename, out stream);
            }
            else
            {
                return CreateStreamAPI29AndAbove(filename, out androidUri, out stream);
            }
        }

        private bool CreateStreamAPI28AndBelow(string filename, out Stream stream)
        {
            stream = null;

            string directory;

#pragma warning disable CS0618 // Type or member is obsolete
            directory = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
#pragma warning restore CS0618 // Type or member is obsolete

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
                    System.Diagnostics.Debug.WriteLine("Failed to delete existing filespec: " + filename);
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                    System.Diagnostics.Debug.Flush();
                    return false;
                }
            }

            try
            {
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

        private bool CreateStreamAPI29AndAbove(string filename, out string androidUri, out Stream stream)
        {
            androidUri = null;
            stream = null;

            string fileNameWithoutExt = Path.ChangeExtension(filename, null);
            string fileExtOnly = Path.GetExtension(filename);

            ContentValues values = new ContentValues();
            ContentResolver contentResolver = CrossCurrentActivity.Current.AppContext.ContentResolver;

            values.Put(Android.Provider.MediaStore.IMediaColumns.Title, filename);
            if (fileExtOnly.Contains(ExportDataHelper.CsvExtension))
            {
                values.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, ExportDataHelper.CsvMimeType);
            }
            else if (fileExtOnly.Contains(ExportDataHelper.ExcelExtension))
            {
                values.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, ExportDataHelper.ExcelMimeType);
            }

            values.Put(Android.Provider.MediaStore.Downloads.InterfaceConsts.DisplayName, fileNameWithoutExt);

            Android.Net.Uri newUri;

            try
            {
                newUri = contentResolver.Insert(Android.Provider.MediaStore.Downloads.ExternalContentUri, values);
                if (newUri == null)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to get URI back from content resolver.");
                    return false;
                }

                androidUri = newUri.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to get data back from content resolver. Filename: " + filename);
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                System.Diagnostics.Debug.Flush();
                return false;
            }

            try
            {
                stream = contentResolver.OpenOutputStream(newUri);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed file write stream output: " + filename);
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                System.Diagnostics.Debug.Flush();
                return false;
            }

            return true;
        }
    }
}
