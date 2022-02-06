using FileHelpers;
using System;

namespace Export.Model
{
    //// The below annotation is for FileHelpers
    [DelimitedRecord("|")]
    public class Example
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public int Counts { get; set; }
        public string Hold { get; set; }
        public string Sets { get; set; }
        public DateTime RunDate { get; set; }
    }
}
