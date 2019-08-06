using System;
using System.Collections.Generic;
using Export.Model;
using OfficeOpenXml;

namespace Export.Helper
{
    public class GenerateSpeadsheet
    {
        public bool CreateSpeadsheet(string filespec, string worksheetName, IList<Example> exampleList)
        {
            try
            {
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet sht = pck.Workbook.Worksheets.Add(worksheetName);
                    sht.Cells["a1"].LoadFromCollection(exampleList, true);
                    pck.SaveAs(new System.IO.FileInfo(filespec));
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CreateSpeadsheet() filespec: " + filespec +
                    "  - exception: " + ex.Message);
                System.Diagnostics.Debug.Flush();
                return false;
            }
        }
    }
}
