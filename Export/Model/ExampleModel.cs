using System;
using System.Collections.Generic;

namespace Export.Model
{
    public class ExampleModel
    {
        public const string EXPORT_FILENAME = "example.xlsx";
        public List<Example> ExportList;

        public ExampleModel()
        {
            ExportList = new List<Example>();
            Example example = new Example
            {
                Id = 1,
                Name = "one",
                Link = "",
                Description = "desc one",
                Counts = 1,
                Hold = "10 secs",
                Sets = "1"
            };
            ExportList.Add(example);

            example = new Example
            {
                Id = 2,
                Name = "two",
                Link = "",
                Description = "desc two",
                Counts = 2,
                Hold = "20 secs",
                Sets = "2"
            };
            ExportList.Add(example);

            example = new Example
            {
                Id = 3,
                Name = "three",
                Link = "",
                Description = "desc three",
                Counts = 3,
                Hold = "30 secs",
                Sets = "3"
            };
            ExportList.Add(example);
        }
    }
}