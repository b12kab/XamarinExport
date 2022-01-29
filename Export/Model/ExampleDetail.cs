using FileHelpers;
using System;

namespace Export.Model
{
    [DelimitedRecord("|")]
    public class ExampleDetail
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public DateTime DetailRunDate { get; set; }
    }
}
