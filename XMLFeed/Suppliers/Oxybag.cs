using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XMLFeed.Suppliers
{
    public class Oxybag : Supplier
    {
        public Oxybag(Options opts) : base(opts)
        {
        }

        public override void Transform()
        {
            XmlNodeList items = doc.SelectNodes("/SHOP/SHOPITEM");
            foreach (XmlNode item in items)
            {
                // rename ITEM_ID to CODE, add Prefix
                XmlNode itemId = item.SelectSingleNode("ITEM_ID");
                XmlElement code = doc.CreateElement("CODE");
                code.InnerXml = Prefix + itemId.InnerXml;
                item.ReplaceChild(code, itemId);

                // rename PRODUCTNAME to NAME
                XmlNode productName = item.SelectSingleNode("PRODUCTNAME");
                XmlElement name = doc.CreateElement("NAME");
                name.InnerXml = productName.InnerXml;
                item.ReplaceChild(name, productName);

                // rename DESCRIPTION to SHORT_DESCRIPTION
                XmlNode description = item.SelectSingleNode("DESCRIPTION");
                XmlElement shortDescription = doc.CreateElement("SHORT_DESCRIPTION");
                shortDescription.InnerXml = description.InnerXml;
                item.ReplaceChild(shortDescription, description);

                // rename DESCRIPTION_FULL to DESCRIPTION
                XmlNode descrptionFull = item.SelectSingleNode("DESCRIPTION_FULL");
                description = doc.CreateElement("DESCRIPTION");
                description.InnerXml = descrptionFull.InnerXml;
                item.ReplaceChild(description, descrptionFull);

                // remove URL
                XmlNode url = item.SelectSingleNode("URL");
                if (url != null)
                {
                    item.RemoveChild(url);
                }

                // rename IMGURLx to IMAGE
                XmlNode imagesOld = item.SelectSingleNode("IMAGES");
                XmlElement imagesNew = doc.CreateElement("IMAGES");
                foreach (XmlNode imgurl in imagesOld.ChildNodes)
                {
                    XmlElement image = doc.CreateElement("IMAGE");
                    image.InnerXml = imgurl.InnerXml;
                    imagesNew.AppendChild(image);
                }
                item.AppendChild(imagesNew);
                item.RemoveChild(imagesOld);

                // transform CATEGORYTEXTs to CATEGORIES/CATEGORY
                XmlNodeList categorytexts = item.SelectNodes("CATEGORYTEXT");
                XmlElement cetegories = doc.CreateElement("CATEGORIES");
                foreach (XmlNode categorytext in categorytexts)
                {
                    XmlElement category = doc.CreateElement("CATEGORY");
                    string cattxt = categorytext.InnerXml;
                    category.InnerXml = cattxt;
                    cetegories.AppendChild(category);
                    categorytext.ParentNode.RemoveChild(categorytext);
                }
                item.AppendChild(cetegories);

                // remove node <PRICE>?</PRICE>
                XmlNode price = item.SelectSingleNode("PRICE");
                if (price != null && price.InnerText == "?")
                {
                    item.RemoveChild(price);
                }
            }
        }
    }
}
