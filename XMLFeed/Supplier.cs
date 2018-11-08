using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XMLFeed
{
    public abstract class Supplier
    {
        protected XmlDocument doc = new XmlDocument();

        private readonly string Input;
        private readonly string Output;

        public Supplier(Options opts)
        {
            Input = opts.Input;
            Output = opts.Output;
        }

        public void Load()
        {
            doc.Load(Input);
        }

        public void Save()
        {  
            doc.Save(Output);
        }

        public abstract void Transform();
    }
}
