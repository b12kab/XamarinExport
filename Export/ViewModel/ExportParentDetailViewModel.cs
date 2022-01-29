using Export.Helper;
using Export.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Export.ViewModel
{
    public class ExportParentDetailViewModel : INotifyPropertyChanged
    {
        public INavigation _navigation;
        public ICommand ExportCsvCommand { get; private set; }
        public ICommand ExportExcelCommand { get; private set; }

        public ExportParentDetailViewModel(INavigation navigation)
        {
            _navigation = navigation;
            Info = "";
            ExportCsvCommand = new Command(async () => await ExportData(ExportType.CSV));
            ExportExcelCommand = new Command(async () => await ExportData(ExportType.Excel));
        }

        public enum ExportType { CSV, Excel };

        private ExportDataHelper exportInfo = null;
        private string filename = null;
        private string androidUri;
        private string infoDescription;

        async Task ExportData(ExportType exportType)
        {
            try
            {
                bool exportData = await Application.Current.MainPage.DisplayAlert("Export Data", "Do you want to export the data?", "OK", "Cancel");
                if (exportData)
                {
                    bool permissionGranted = await CheckPermission();
                    if (!permissionGranted)
                    {
                        await Application.Current.MainPage.DisplayAlert("Export Data", "You didn't give permission. If you wish to give permission, please agree to the permission", "OK");
                    }

                    if (exportInfo == null)
                    {
                        exportInfo = new ExportDataHelper();
                    }

                    string timeframe = DateTime.Now.ToString("yyyy_M_dd_hh_mm_ss_fff");

                    filename = ExampleGeneratorHelper.EXPORT_FILENAME;
                    ////filename = ExampleModel.EXPORT_FILENAME + "Master " + DateTime.Now.TimeOfDay.ToString() + ".csv";

                    if (exportType == ExportType.CSV)
                    {
                        filename += "main " + timeframe + "." + ExportDataHelper.CsvExtension;
                    }
                    else if (exportType == ExportType.Excel)
                    {
                        filename += timeframe + "." + ExportDataHelper.ExcelExtension;
                    }

                    bool worked = exportInfo.CreateFileStream(filename, out androidUri);

                    if (worked)
                    {
                        if (!string.IsNullOrWhiteSpace(this.androidUri))
                        {
                            Info = "URI: " + this.androidUri;
                            System.Diagnostics.Debug.WriteLine("Write file - Android URI: " + this.androidUri);
                        }

                        await Application.Current.MainPage.DisplayAlert("Export Data", "Export stream created", "OK");

                        if (exportType == ExportType.CSV)
                        {
                            exportInfo.CreateExampleCSV();
                            System.Diagnostics.Debug.WriteLine("CreateExampleCSV completed");
                        }
                        else if (exportType == ExportType.Excel)
                        {
                            exportInfo.CreateExampleExcel();
                            System.Diagnostics.Debug.WriteLine("CreateExampleExcel completed");
                        }
                        await Application.Current.MainPage.DisplayAlert("Export Data", "File export complete", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Export Data", "Export stream failed", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Export error - exception: " + ex.Message);
                System.Diagnostics.Debug.Flush();
            }
        }

        /// <summary>
        /// Check to see if permissions are needed for Android, where it's API 28 or less
        /// </summary>
        /// <returns>true = permission needed, false = permission not needed</returns>
        private async Task<bool> CheckPermission()
        {
            bool permissionNeeded = DependencyService.Get<IFile>().IsFilePermissionNeeded();

            if (!permissionNeeded)
            {
                return true;
            }

            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (status == PermissionStatus.Granted)
            {
                return true;
            }

            if (Permissions.ShouldShowRationale<Permissions.StorageWrite>())
            {
                await Application.Current.MainPage.DisplayAlert("Export Data", "Without this permission, the file cannot be exported.", "OK");
            }

            status = await Permissions.RequestAsync<Permissions.StorageWrite>();


            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //---------------------------------------------------------------------
        //---------------------------------------------------------------------

        public string Info
        {
            get => infoDescription;
            set
            {
                infoDescription = value;
                NotifyPropertyChanged("Info");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
