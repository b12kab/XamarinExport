using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Export
{
    public partial class ViewMenu : ContentPage
    {
        public ViewMenu()
        {
            InitializeComponent();
            BindingContext = new ExportViewModel(Navigation);
            RegisterMesssages();
        }

        private void RegisterMesssages()
        {
            MessagingCenter.Subscribe<ExportViewModel>(this, "DataExportedSuccessfully", (senderViewModel) =>
            {
                if (senderViewModel != null)
                {
                    DisplayAlert("Info", "Data exported Successfully.", "OK");
                }
            });

            MessagingCenter.Subscribe<ExportViewModel>(this, "DataExportedPermissionGiven", (senderViewModel) =>
            {
                DisplayAlert("Info", "Please restart the app to allow this change", "OK");
            });

            MessagingCenter.Subscribe<ExportViewModel>(this, "DataExportedPermissionDenied", (senderViewModel) =>
            {
                DisplayAlert("Error", "You have denied the write permission", "OK");
            });

            MessagingCenter.Subscribe<ExportViewModel>(this, "DataExportFailure", (senderViewModel) =>
            {
                DisplayAlert("Error", "Failed to export the data", "OK");
            });

            MessagingCenter.Subscribe<ExportViewModel>(this, "DataExportedFirstPermissionGiven", (senderViewModel) =>
            {
                DisplayAlert("Info", "Close this app and retry to export the file", "OK");
            });

            MessagingCenter.Subscribe<ExportViewModel>(this, "NoDataToExport", (senderViewModel) =>
            {
                DisplayAlert("Warning !", "No data to export.", "OK");
            });

            MessagingCenter.Subscribe<ExportViewModel>(this, "DataDownloadFolderFailed", (senderViewModel) =>
            {
                DisplayAlert("Info", "Set download directory failed.", "OK");
            });

            MessagingCenter.Subscribe<ExportViewModel>(this, "DataDownloadFolderFailed", (senderViewModel) =>
            {
                DisplayAlert("Info", "Set download directory failed.", "OK");
            });
        }
    }
}
