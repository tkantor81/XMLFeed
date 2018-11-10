using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMLFeed
{
    public class Options
    {
        [Option("supplier", Required = true, HelpText = "Supplier - TIPTRADE | MATYSKA | MATYSKA_STOCK")]
        public string Supplier { get; set; }
        
        [Option("input", Required = true, HelpText = "Input XML Feed file")]
        public string Input { get; set; }

        [Option("extension", Required = false, HelpText = "Extension XML Feed file")]
        public string Extension { get; set; }

        [Option("prefix", Required = false, HelpText = "Code prefix")]
        public string Prefix { get; set; }

        [Option("output", Required = true, HelpText = "Output XML Feed file")]
        public string Output { get; set; }
    }
}
