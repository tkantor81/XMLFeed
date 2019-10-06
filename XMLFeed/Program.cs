using CommandLine;
using System;
using XMLFeed.Suppliers;

namespace XMLFeed
{
    class Program
    {
        static void Main(string[] args)
        {
            Options opts = new Options();
            Parser.Default.ParseArgumentsStrict(args, opts, () => 
            {
                Console.WriteLine("Example arguments: --supplier=MATYSKA --input=\"https://www.puzzle-puzzle.cz/xml/voc-feed.xml?u=meta-shop&p=wa45bp\" --extension=\"https://www.puzzle-puzzle.cz/xml/voc-dostupnost.xml?u=meta-shop&p=wa45bp\" --prefix=PUZ --output=output.xml");
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
                case "MATYSKA_STOCK":
                    supp = new MatyskaStock(opts);
                    break;
                case "CORFIX":
                    supp = new Corfix(opts);
                    break;
                case "OXYBAG":
                    supp = new Oxybag(opts);
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
                supp.Extend();
                supp.Save();
                Console.WriteLine("Done");
            }
        }
    }
}
