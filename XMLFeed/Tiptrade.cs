using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XMLFeed
{
    public class Tiptrade : Supplier
    {
        public Tiptrade(Options opts) : base(opts)
        {
        }

        public override void Transform()
        {
            XmlNodeList items = doc.SelectNodes("/SHOP/SHOPITEM");
            foreach (XmlNode item in items)
            {
                // rename PRODUCT to NAME
                XmlNode product = item.SelectSingleNode("PRODUCT");
                XmlNode name = doc.CreateElement("NAME");
                name.InnerXml = product.InnerXml;
                item.InsertBefore(name, product);
                item.RemoveChild(product);

                // remove URL
                XmlNode url = item.SelectSingleNode("URL");
                item.RemoveChild(url);

                // transform IMGURL to IMAGES/IMAGE
                XmlNode imgurl = item.SelectSingleNode("IMGURL");
                XmlNode images = doc.CreateElement("IMAGES");
                XmlNode image = doc.CreateElement("IMAGE");
                image.InnerXml = imgurl.InnerXml;
                images.AppendChild(image);
                item.InsertBefore(images, imgurl);
                item.RemoveChild(imgurl);

                // transform CATEGORYTEXT to CATEGORIES/CATEGORY
                XmlNode categorytext = item.SelectSingleNode("CATEGORYTEXT");
                XmlNode cetegories = doc.CreateElement("CATEGORIES");
                XmlNode category = doc.CreateElement("CATEGORY");
                category.InnerXml = categorytext.InnerXml.Replace('|', '>');
                cetegories.AppendChild(category);
                item.InsertBefore(cetegories, categorytext);
                item.RemoveChild(categorytext);

                // remove PRODUCTNO
                XmlNode productno = item.SelectSingleNode("PRODUCTNO");
                item.RemoveChild(productno);

                // transform DELIVERY_DATE to AVAILABILITY
                XmlNode deliveryDate = item.SelectSingleNode("DELIVERY_DATE");
                XmlNode availability = doc.CreateElement("AVAILABILITY");
                
                /*
                    XML	    Jejich web      Nas web
                    ---------------------------------
                    nic     Vyprodáno       Vyprodáno
                    0       Skladem         2 - 4 dny
                    1       ?               3 - 5 dnů
                    2       ?               4 - 6 dnů
                    3       ?               5 - 7 dnů
                    4       3 - 5 dnů       6 - 8 dnů
                    5       Na cestě        Na cestě
                    jinak   ?               Neznámá
                */
                if (Int32.TryParse(deliveryDate.InnerXml, out int delivery))
                {
                    switch (delivery)
                    {
                        case 0:
                            availability.InnerXml = "2 - 4 dny";
                            break;
                        case 1:
                            availability.InnerXml = "3 - 5 dnů";
                            break;
                        case 2:
                            availability.InnerXml = "4 - 6 dnů";
                            break;
                        case 3:
                            availability.InnerXml = "5 - 7 dnů";
                            break;
                        case 4:
                            availability.InnerXml = "6 - 8 dnů";
                            break;
                        case 5:
                            availability.InnerXml = "Na cestě";
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
                
                item.InsertBefore(availability, deliveryDate);
                item.RemoveChild(deliveryDate);

                // fill EAN if empty
                XmlNode ean = item.SelectSingleNode("EAN");
                if (ean.InnerXml == "")
                {
                    ean.InnerXml = "0";
                }
            }
        }
    }
}
