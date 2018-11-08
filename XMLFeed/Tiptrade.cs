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
                XmlElement name = doc.CreateElement("NAME");
                name.InnerXml = product.InnerXml;
                item.ReplaceChild(name, product);

                // remove URL
                XmlNode url = item.SelectSingleNode("URL");
                item.RemoveChild(url);

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
                    category.InnerXml = categorytext.InnerXml.Replace('|', '>');;
                    cetegories.AppendChild(category);
                    categorytext.ParentNode.RemoveChild(categorytext);
                }
                item.AppendChild(cetegories);

                // remove PRODUCTNO
                XmlNode productno = item.SelectSingleNode("PRODUCTNO");
                item.RemoveChild(productno);

                // transform DELIVERY_DATE to AVAILABILITY
                XmlNode deliveryDate = item.SelectSingleNode("DELIVERY_DATE");
                XmlElement availability = doc.CreateElement("AVAILABILITY");
                /*
                    XML	      Their web                 Mine web
                    ---------------------------------------------------------
                    empty     Vyprodáno                 Vyprodáno
                    0         Skladem                   2 - 4 dny
                    1         ?                         3 - 5 dnů
                    2         ?                         4 - 6 dnů
                    3         ?                         5 - 7 dnů
                    4         3 - 5 dnů                 6 - 8 dnů
                    5         Na cestě                  Na cestě
                    30        Momentálně nedostupné     Momentálně nedostupné 
                    else      ?                         Neznámá
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
                        case 30:
                            availability.InnerXml = "Momentálně nedostupné";
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
