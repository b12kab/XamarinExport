using System;
using System.Collections.Generic;
using Export.ViewModel;
using Xamarin.Forms;

namespace Export
{
    public partial class ViewMenu : ContentPage
    {
        public ViewMenu()
        {
            InitializeComponent();
            BindingContext = new ExportParentDetailViewModel(Navigation);
            RegisterMesssages();
        }

        private void RegisterMesssages()
        {
            MessagingCenter.Subscribe<ExportParentDetailViewModel>(this, "DataExportedSuccessfully", (senderViewModel) =>
            {
                if (senderViewModel != null)
                {
                    DisplayAlert("Info", "Data exported Successfully.", "OK");
                }
            });

            MessagingCenter.Subscribe<ExportParentDetailViewModel>(this, "DataExportedPermissionGiven", (senderViewModel) =>
            {
                DisplayAlert("Info", "Please restart the app to allow this change", "OK");
            });

            MessagingCenter.Subscribe<ExportParentDetailViewModel>(this, "DataExportedPermissionDenied", (senderViewModel) =>
            {
                DisplayAlert("Error", "You have denied the write permission", "OK");
            });

            MessagingCenter.Subscribe<ExportParentDetailViewModel>(this, "DataExportFailure", (senderViewModel) =>
            {
                DisplayAlert("Error", "Failed to export the data", "OK");
            });

            MessagingCenter.Subscribe<ExportParentDetailViewModel>(this, "DataExportedFirstPermissionGiven", (senderViewModel) =>
            {
                DisplayAlert("Info", "Close this app and retry to export the file", "OK");
            });

            MessagingCenter.Subscribe<ExportParentDetailViewModel>(this, "NoDataToExport", (senderViewModel) =>
            {
                DisplayAlert("Warning !", "No data to export.", "OK");
            });

            MessagingCenter.Subscribe<ExportParentDetailViewModel>(this, "DataDownloadFolderFailed", (senderViewModel) =>
            {
                DisplayAlert("Info", "Set download directory failed.", "OK");
            });

            MessagingCenter.Subscribe<ExportParentDetailViewModel>(this, "DataDownloadFolderFailed", (senderViewModel) =>
            {
                DisplayAlert("Info", "Set download directory failed.", "OK");
            });
        }
    }
}
