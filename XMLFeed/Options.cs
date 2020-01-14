using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMLFeed
{
    public class Options
    {
        [Option('s', "supplier", Required = true, HelpText = "Supplier - TIPTRADE | MATYSKA | MATYSKA_STOCK")]
        public string Supplier { get; set; }
        
        [Option('i', "input", Required = true, HelpText = "Input XML Feed file")]
        public string Input { get; set; }

        [Option('e', "extension", Required = false, HelpText = "Extension XML Feed file")]
        public string Extension { get; set; }

        [Option('p', "prefix", Required = false, HelpText = "Code prefix")]
        public string Prefix { get; set; }

        [Option('m', "minprice", Required = false, HelpText = "Minimum transformed item price")]
        public int MinPrice { get; set; } = 0;

        [Option('o', "output", Required = true, HelpText = "Output XML Feed file")]
        public string Output { get; set; }
    }
}
