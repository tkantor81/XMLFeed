using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XMLFeed
{
    public class Matyska : Supplier
    {
        public Matyska(Options opts) : base(opts)
        {
        }

        public override void Transform()
        {
            XmlNodeList items = doc.SelectNodes("/SHOP/SHOPITEM");
            foreach (XmlNode item in items)
            {
                // rename ITEM_ID to CODE
                XmlNode itemId = item.SelectSingleNode("ITEM_ID");
                XmlElement code = doc.CreateElement("CODE");
                code.InnerXml = itemId.InnerXml;
                item.ReplaceChild(code, itemId);

                // rename PRODUCT to NAME
                XmlNode product = item.SelectSingleNode("PRODUCT");
                XmlElement name = doc.CreateElement("NAME");
                name.InnerXml = product.InnerXml;
                item.ReplaceChild(name, product);

                // transform PARAMs to TEXT_PROPERTIES/TEXT_PROPERTY
                XmlNodeList paramss = item.SelectNodes("PARAM");
                XmlElement textProperties = doc.CreateElement("TEXT_PROPERTIES");
                foreach (XmlNode param in paramss)
                {
                    XmlElement textProperty = doc.CreateElement("TEXT_PROPERTY");
                    XmlElement propName = doc.CreateElement("NAME");
                    propName.InnerText = param.FirstChild.InnerXml;
                    textProperty.AppendChild(propName);
                    XmlElement propValue = doc.CreateElement("VALUE");
                    propValue.InnerText = param.LastChild.InnerXml;
                    textProperty.AppendChild(propValue);
                    textProperties.AppendChild(textProperty);
                    param.ParentNode.RemoveChild(param);
                }
                item.AppendChild(textProperties);

                // transform IMGURLs to IMAGES/IMAGE
                XmlNodeList imgurls = item.SelectNodes("IMGURL");
                XmlElement images = doc.CreateElement("IMAGES");
                foreach (XmlNode imgurl in imgurls)
                {
                    XmlElement image = doc.CreateElement("IMAGE");
                    image.InnerXml = imgurl.InnerXml;
                    images.AppendChild(image);
                    imgurl.ParentNode.RemoveChild(imgurl);
                }
                item.AppendChild(images);

                // transform CATEGORYTEXTs to CATEGORIES/CATEGORY
                XmlNodeList categorytexts = item.SelectNodes("CATEGORYTEXT");
                XmlElement cetegories = doc.CreateElement("CATEGORIES");
                foreach (XmlNode categorytext in categorytexts)
                {
                    XmlElement category = doc.CreateElement("CATEGORY");
                    category.InnerXml = categorytext.InnerXml.Replace(@"&amp;gt;", ">");;
                    cetegories.AppendChild(category);
                    categorytext.ParentNode.RemoveChild(categorytext);
                }
                item.AppendChild(cetegories);

                // transform DELIVERY_DATE to AVAILABILITY
                XmlNode deliveryDate = item.SelectSingleNode("DELIVERY_DATE");
                XmlElement availability = doc.CreateElement("AVAILABILITY");
                /*
                    XML	      Their web     Mine web
                    ----------------------------------
                    empty     Vyprodáno     Vyprodáno
                    0         Skladem       2 - 4 dny
                    else      ?             Neznámá
                */
                if (Int32.TryParse(deliveryDate.InnerXml, out int delivery))
                {
                    switch (delivery)
                    {
                        case 0:
                            availability.InnerXml = "2 - 4 dny";
                            break;
                        default:
                            availability.InnerXml = "Neznámá";
                            break;
                    }
                }
                else
                {
                    availability.InnerXml = "Vyprodáno";
                }
                item.ReplaceChild(availability, deliveryDate);
            }
        }
    }
}
