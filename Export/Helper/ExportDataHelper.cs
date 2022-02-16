using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Export.Model;
using FileHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Export.Helper
{
    public class ExportDataHelper
    {
        public static readonly string ExcelExtension = "xlsx";
        public static readonly string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public static readonly string CsvExtension = "csv";
        public static readonly string CsvMimeType = "text/comma-separated-values"; // note: iOS sees it as "text/csv

        private OpenXMLHelper openXMLHelper = null;
        private Stream stream = null;
        private List<Example> exampleList = null;
        private List<ExampleDetail> exampleDetailList = null;

        public ExportDataHelper()
        {
            ExampleGeneratorHelper exampleModel = new ExampleGeneratorHelper();
            this.openXMLHelper = new OpenXMLHelper();
            this.exampleList = exampleModel.ExportList;
            this.exampleDetailList = exampleModel.ExportDetailList;
        }

        public bool CreateFileStream(string filename, out string androidUri)
        {
            return DependencyService.Get<IFile>().CreateOutputStream(filename, out androidUri, out this.stream);
        }

        public void CreateExampleCSV()
        {
            try
            {
                var engine = new FileHelperEngine<Example>();

                StreamWriter streamWriter = new StreamWriter(this.stream);
                engine.WriteStream(streamWriter, this.exampleList);

                streamWriter.Close();
                streamWriter.Dispose();

                this.stream.Close();
                this.stream.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to write Example data to stream: " + ex.ToString());
            }
        }

        public void CreateExampleDetailCSV()
        {
            try
            {
                var engine = new FileHelperEngine<ExampleDetail>();

                StreamWriter streamWriter = new StreamWriter(this.stream);
                engine.WriteStream(streamWriter, this.exampleDetailList);

                streamWriter.Close();
                streamWriter.Dispose();

                this.stream.Close();
                this.stream.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to write ExampleDetail data to stream: " + ex.ToString());
            }
        }

        public void CreateExampleExcel()
        {
            try
            {
                var cacheDir = FileSystem.CacheDirectory;

                var cacheFile = Path.Combine(cacheDir, Path.GetRandomFileName());
                string cacheFileWithoutExt = Path.ChangeExtension(cacheFile, null);
                string cacheFileWithExcelExt = cacheFileWithoutExt + "." + ExportDataHelper.ExcelExtension;

                FileStream fileStream = new(cacheFileWithExcelExt, FileMode.CreateNew, FileAccess.ReadWrite);

                using (SpreadsheetDocument document = SpreadsheetDocument.Create(fileStream, SpreadsheetDocumentType.Workbook))
                {
                    this.PopulateExampleSpreadsheet(document);

                    document.Save();
                    document.Close();
                    document.Dispose();
                }

                // rewind file pointer to the start of file
                fileStream.Position = 0;

                // copy to output stream
                fileStream.CopyTo(this.stream);

                fileStream.Close();
                fileStream.Dispose();

                this.stream.Close();
                this.stream.Dispose();

                // Delete temp file
                File.Delete(cacheFileWithExcelExt);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CreateExampleExcel - Failed to write Excel Example data to stream: " + ex.ToString());
            }
        }

        /// <summary>
        ////// From https://developpaper.com/net-core-uses-openxml-to-export-and-import-excel/
        //////      https://stackoverflow.com/questions/9120544/openxml-multiple-sheets
        /// </summary>
        private void PopulateExampleSpreadsheet(SpreadsheetDocument document)
        {
            try
            {                
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                //WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                //workbookStylesPart.Stylesheet = openXMLHelper.GenerateNewStylesheet();
                //workbookStylesPart.Stylesheet.Save();

                this.CreateDataSheet(ref document, ref workbookPart, ref sheets, 1, "Example data - 1", CreateExampleHeader, CreateExampleRows);
                this.CreateDataSheet(ref document, ref workbookPart, ref sheets, 2, "Example Detail - 2", CreateExampleDetailHeader, CreateExampleDetailRows);

                workbookPart.Workbook.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PopulateSpreadsheet - Failed to create Excel data: " + ex.ToString());
            }
        }

        /// <summary>
        /// Create a new sheet
        /// </summary>
        private bool CreateDataSheet(ref SpreadsheetDocument document, ref WorkbookPart workbookPart, ref Sheets sheets, uint sheetId, string sheetName, Func<Row> createHeader, Func<List<Row>> createData)
        {
            try
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheet newSheet = new()
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = sheetId,
                    Name = sheetName
                };

                sheets.Append(newSheet);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                sheetData.Append(createHeader());
                sheetData.Append(createData());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CreateDataSheet - Failed to create Exercise sheet data: " + ex.ToString());
                return true;
            }

            return false;
        }

        /// <summary>
        /// https://dotscrapbook.wordpress.com/2011/08/26/creating-and-streaming-an-openxml-spreadsheet-using-the-net-sdk-and-asp-net-mvc/
        /// </summary>
        private Row CreateExampleHeader()
        {
            Row row = new Row();
            row.AppendChild(openXMLHelper.AddCell("Id", 1));
            row.AppendChild(openXMLHelper.AddCell("Name", 1));
            row.AppendChild(openXMLHelper.AddCell("Link", 1));
            row.AppendChild(openXMLHelper.AddCell("Description", 1));
            row.AppendChild(openXMLHelper.AddCell("Counts", 1));
            row.AppendChild(openXMLHelper.AddCell("Hold", 1));
            row.AppendChild(openXMLHelper.AddCell("Sets", 1));
            row.AppendChild(openXMLHelper.AddCell("RunDate", 1));

            return row;
        }

        private List<Row> CreateExampleRows()
        {
            List<Row> rows = new List<Row>();
            var data = this.GetExampleData();

            foreach (var item in data)
            {
                Row row = new Row();
                row.AppendChild(openXMLHelper.AddCell(item.Id));
                row.AppendChild(openXMLHelper.AddCell(item.Name));
                row.AppendChild(openXMLHelper.AddCell(item.Link));
                row.AppendChild(openXMLHelper.AddCell(item.Description));
                row.AppendChild(openXMLHelper.AddCell(item.Counts));
                row.AppendChild(openXMLHelper.AddCell(item.Hold));
                row.AppendChild(openXMLHelper.AddCell(item.Sets));
                row.AppendChild(openXMLHelper.AddCell(item.RunDate));

                rows.Add(row);
            }

            return rows;
        }

        private List<ExampleDTO> GetExampleData()
        {
            var exampleInfo = from s in this.exampleList
                              select new ExampleDTO
                              {
                                  Id = s.Id,
                                  Name = s.Name ?? string.Empty,
                                  Link = s.Link ?? string.Empty,
                                  Description = s.Description ?? string.Empty,
                                  Counts = s.Counts,
                                  Hold = s.Hold ?? string.Empty,
                                  Sets = s.Sets ?? string.Empty,
                                  RunDate = s.RunDate.ToString()
                              };

            return exampleInfo.ToList();
        }

        private Row CreateExampleDetailHeader()
        {
            Row row = new Row();
            row.AppendChild(openXMLHelper.AddCell("Id", 1));
            row.AppendChild(openXMLHelper.AddCell("ParentId", 1));
            row.AppendChild(openXMLHelper.AddCell("Name", 1));
            row.AppendChild(openXMLHelper.AddCell("DetailRunDate", 1));
            row.AppendChild(openXMLHelper.AddCell("Generate", 1));

            return row;
        }

        private List<Row> CreateExampleDetailRows()
        {
            List<Row> rows = new List<Row>();
            var data = this.exampleDetailList;

            foreach (var item in data)
            {
                Row row = new Row();
                row.AppendChild(openXMLHelper.AddCell(item.Id));
                row.AppendChild(openXMLHelper.AddCell(item.ParentId));
                row.AppendChild(openXMLHelper.AddCell(item.Name));
                row.AppendChild(openXMLHelper.AddCell(item.DetailRunDate, 2));
                row.AppendChild(openXMLHelper.AddCell(item.Generate));

                rows.Add(row);
            }

            return rows;
        }
    }
}
