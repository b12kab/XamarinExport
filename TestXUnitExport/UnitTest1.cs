using System;
using System.IO;
using Export.Helper;
using Export.Model;
using Xunit;

namespace TestXUnitExport
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            ExampleModel example = new ExampleModel();

            GenerateSpeadsheet generateSpeadsheet = new GenerateSpeadsheet();

            bool worked = generateSpeadsheet.CreateSpeadsheet(GetFilename(), "test", example.ExportList);

            Assert.True(worked);
        }

        public string GetFilename()
        {
            string thisPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var path = Path.Combine(thisPath, "example.xlsx");

            FileInfo newFile = new FileInfo(path);
            if (newFile.Exists)
            {
                newFile.Delete();  // ensures we create a new workbook
            }

            return path;
        }
    }
}
