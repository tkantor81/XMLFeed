using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XMLFeed
{
    public class Transformation
    {
        public static void PrependPrefixToCODE(XmlNode item, string prefix)
        {
            XmlNode code = item.SelectSingleNode("CODE");
            code.InnerXml = prefix + code.InnerXml;
        }

        public static void FillEmptyEAN(XmlNode item)
        {
            XmlNode ean = item.SelectSingleNode("EAN");
            if (ean.InnerXml == "")
            {
                ean.InnerXml = "0";
            }
        }

        public static void RenameTag()
        {

        }

        public static void RemoveTag()
        {

        }
    }
}
