using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Export.Helper;
using Export.Model;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace Export
{
    public class ExportViewModel : INotifyPropertyChanged
    {
        public INavigation _navigation;
        public ICommand ExportCommand { get; private set; }

        public ExportViewModel(INavigation navigation)
        {
            _navigation = navigation;

            ExampleModel exampleModel = new ExampleModel();
            ExampleList = exampleModel.ExportList;

            ExportCommand = new Command(async () => await ExportData());
        }

        private static bool permissionDeniedOnce = false;
        private static bool initialPermission = true;
        private bool exportData = false;

        async Task ExportData()
        {
            try
            {
                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

                if (storageStatus != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                    storageStatus = results[Permission.Storage];

                    if (!permissionDeniedOnce && storageStatus == PermissionStatus.Denied)
                    {
                        permissionDeniedOnce = true;
                    }

                    if (!permissionDeniedOnce && storageStatus == PermissionStatus.Granted)
                    {
                        if (initialPermission)
                        {
                            MessagingCenter.Send(this, "DataExportedFirstPermissionGiven");
                        }
                        exportData = true;
                    }

                    if (permissionDeniedOnce && storageStatus == PermissionStatus.Granted)
                    {
                        //MessagingCenter.Send(this, "DataExportedPermissionGiven");
                        exportData = true;
                    }

                    if (permissionDeniedOnce && storageStatus == PermissionStatus.Denied)
                    {
                        MessagingCenter.Send(this, "DataExportedPermissionDenied");
                    }
                }
                else
                {
                    exportData = true;
                }

                if (exportData)
                {
                    ProcessExportData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Export error - exception: " + ex.Message);
                System.Diagnostics.Debug.Flush();
            }
        }
        const string WORKSHEET_NAME = "example data";

        bool ProcessExportData()
        {
            bool worked = false;

            if (ExampleList.Count > 0)
            {
                try
                {
                    string dirName = DependencyService.Get<IFile>().GetDirectory();
                    System.Diagnostics.Debug.WriteLine("Excel directory name: " + dirName);
                    System.Diagnostics.Debug.Flush();

                    string filespec = DependencyService.Get<IFile>().GenerateFilespec(dirName, ExampleModel.EXPORT_FILENAME);

                    System.Diagnostics.Debug.WriteLine("Excel filespec: " + filespec);
                    System.Diagnostics.Debug.Flush();

                    GenerateSpeadsheet generateSpeadsheet = new GenerateSpeadsheet();

                    worked = generateSpeadsheet.CreateSpeadsheet(filespec, WORKSHEET_NAME, ExampleList);

                    if (worked)
                    {
                        worked = DependencyService.Get<IFile>().SetDownload(dirName, ExampleModel.EXPORT_FILENAME, "example desc");
                        if (worked)
                        {
                            MessagingCenter.Send(this, "DataExportedSuccessfully");
                            System.Diagnostics.Debug.WriteLine("Export successful.");
                            System.Diagnostics.Debug.Flush();
                        }
                        else
                        {
                            MessagingCenter.Send(this, "DataDownloadFolderFailed");
                            System.Diagnostics.Debug.WriteLine("Set download directory failed.");
                            System.Diagnostics.Debug.Flush();
                        }
                    }
                    else
                    {
                        MessagingCenter.Send(this, "DataExportFailure");
                        System.Diagnostics.Debug.WriteLine("Create example spreadsheet failed");
                        System.Diagnostics.Debug.Flush();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                    System.Diagnostics.Debug.Flush();
                }
            }
            else
            {
                MessagingCenter.Send(this, "NoDataToExport");
                worked = true;
            }

            return worked;
        }

        //---------------------------------------------------------------------
        //---------------------------------------------------------------------

        private List<Example> exampleList;
        public List<Example> ExampleList
        {
            get => exampleList;
            set
            {
                if (value != null)
                {
                    exampleList = value;
                    NotifyPropertyChanged("Example");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
