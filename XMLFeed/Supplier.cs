using System;
using System.Collections.Generic;
using System.Text;

namespace XMLFeed
{
    public abstract class Supplier
    {
        public string Input { get; set; }
        public string Output { get; set; }

        public Supplier(Options opts)
        {
            Input = opts.Input;
            Output = opts.Output;
        }

        public abstract void Translate();
    }
}
