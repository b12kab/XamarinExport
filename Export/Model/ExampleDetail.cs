﻿using FileHelpers;
using System;

namespace Export.Model
{
    //// The below annotation is for FileHelpers
    [DelimitedRecord("|")]
    public class ExampleDetail
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public DateTime DetailRunDate { get; set; }
        public bool Generate;
    }
}
