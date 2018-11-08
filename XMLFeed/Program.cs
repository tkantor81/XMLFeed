﻿using CommandLine;
using System;

namespace XMLFeed
{
    class Program
    {
        static void Main(string[] args)
        {
            Options opts = new Options();
            Parser.Default.ParseArgumentsStrict(args, opts, () => 
            {
                Console.WriteLine("Example arguments: --supplier=TIPTRADE --input=\"https://www.levne-povleceni.cz/tiptrade_products.xml\" --output=output.xml");
                Environment.Exit(1);
            });

            Supplier supp = null;
            switch (opts.Supplier)
            {
                case "TIPTRADE":
                    supp = new Tiptrade(opts);
                    break;
                case "MATYSKA":
                    supp = new Matyska(opts);
                    break;
                default:
                    Console.WriteLine("Unsupported supplier");
                    Environment.Exit(1);
                    break;
            }
            if (supp != null)
            {
                supp.Load();
                supp.Transform();
                supp.Save();
                Console.WriteLine("Done");
            }
        }
    }
}
