using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XMLFeed
{
    public abstract class Supplier
    {
        protected XmlDocument doc = new XmlDocument();
        protected XmlDocument ext = new XmlDocument();

        private readonly string Input;
        private readonly string Extension;
        protected readonly string Prefix;
        private readonly string Output;

        public Supplier(Options opts)
        {
            Input = opts.Input;
            Extension = opts.Extension;
            Prefix = opts.Prefix;
            Output = opts.Output;
        }

        public void Load()
        {
            doc.Load(Input);
            if (Extension != null)
            {
                ext.Load(Extension);
            }
        }

        public void Save()
        {  
            doc.Save(Output);
        }

        public abstract void Transform();
        
        public virtual void Extend()
        { 
        }
    }
}
