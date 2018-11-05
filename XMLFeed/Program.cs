using CommandLine;
using System;

namespace XMLFeed
{
    class Program
    {
        static void Main(string[] args)
        {
            Options opts = new Options();
            Parser.Default.ParseArgumentsStrict(args, opts, () => Console.WriteLine("Example arguments: --supplier=TIPTRADE --input=\"https://www.levne-povleceni.cz/tiptrade_products.xml\" --output=output.xml"));

            switch (opts.Supplier)
            {
                case "TIPTRADE":
                    var supp = new Tiptrade(opts);
                    //TODO
                    break;
                default:
                    Console.WriteLine("Unknown supplier");
                    break;
            }
        }
    }
}
