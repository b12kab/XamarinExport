using System;
using System.Collections.Generic;

namespace Export.Model
{
    public class ExampleGeneratorHelper
    {
        public const string EXPORT_FILENAME = "example ";
        public List<Example> ExportList;
        public List<ExampleDetail> ExportDetailList;

        public ExampleGeneratorHelper()
        {
            ExportList = new List<Example>();
            ExportDetailList = new List<ExampleDetail>();

            Example example = new Example
            {
                Id = 1,
                Name = "one",
                Link = "http://example.com/" + 1,
                Description = "desc one",
                Counts = 1,
                Hold = "10 secs",
                Sets = "1",
                RunDate = DateTime.Now
            };
            ExportList.Add(example);

            ExampleDetail exampleDetail = new ExampleDetail
            {
                Id = 1,
                ParentId = 1,
                Name = "detail one",
                DetailRunDate = DateTime.Now,
                Generate = true
            };
            ExportDetailList.Add(exampleDetail);

            example = new Example
            {
                Id = 2,
                Name = "two",
                Link = "http://example.com/" + 2,
                Description = "desc two",
                Counts = 2,
                Hold = "20 secs",
                Sets = "2",
                RunDate = DateTime.Now
            };
            ExportList.Add(example);

            exampleDetail = new ExampleDetail
            {
                Id = 2,
                ParentId = 2,
                Name = "detail two",
                DetailRunDate = DateTime.Now,
                Generate = true
            };
            ExportDetailList.Add(exampleDetail);

            example = new Example
            {
                Id = 3,
                Name = "three",
                Link = "http://example.com/" + 3,
                Description = "desc three",
                Counts = 3,
                Hold = "30 secs",
                Sets = "3",
                RunDate = DateTime.Now,
            };
            ExportList.Add(example);

            exampleDetail = new ExampleDetail
            {
                Id = 3,
                ParentId = 3,
                Name = "detail three",
                DetailRunDate = DateTime.Now,
                Generate = false
            };
            ExportDetailList.Add(exampleDetail);
        }
    }
}