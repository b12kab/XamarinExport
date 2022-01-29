# Export data to Excel with Xamarin Forms

### Version 2 - see x<p>
Due to Android API 29 changes noted [here](https://keithbeattyblog.wordpress.com/2021/09/20/using-xamarin-android-with-mediastore-part-1/), [here](https://keithbeattyblog.wordpress.com/2021/11/14/using-xamarin-android-with-mediastore-part-2/) and [here](https://keithbeattyblog.wordpress.com/2021/12/26/using-xamarin-android-with-mediastore-part-3/), the export won't work as noted in version 1 (see below). Because of these permission changes, export data for Android won't work as it used to work. Also, the rights for [EPPlus](https://www.nuget.org/packages/EPPlus/) have changed after version 4.5.3.3. As such, this change had to switch away from EPPlus to Microsoft's [DocumentFormat.OpenXml](https://www.nuget.org/packages/DocumentFormat.OpenXml/). 

### Version 1 - see [blog article](https://keithbeattyblog.wordpress.com/2020/01/11/xamarin-exporting-data-to-excel/).<p>
To get the source for this version 1 [use tag 1.0 here](https://github.com/b12kab/XamarinExport/tags) and use the zip download. 

## License
The source for this is released under the MIT license. 
