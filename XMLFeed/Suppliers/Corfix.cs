using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace XMLFeed.Suppliers
{
    public class Corfix : Supplier
    {
        public Corfix(Options opts) : base(opts)
        {
        }

        public override void Transform()
        {
            // rename CATALOG to SHOP
            XmlNode catalog = doc.SelectSingleNode("CATALOG");
            XmlElement shop = doc.CreateElement("SHOP");
            shop.InnerXml = catalog.InnerXml.Replace("ITEM>", "SHOPITEM>");
            doc.ReplaceChild(shop, catalog);

            XmlNodeList items = doc.SelectNodes("/SHOP/SHOPITEM");
            foreach (XmlNode item in items)
            {
                // remove all items where exists <STOCK>false</STOCK>
                XmlNode stock = item.SelectSingleNode("STOCK");
                if (stock.InnerText == "false")
                {
                    item.ParentNode.RemoveChild(item);
                    continue;
                }

                // rename PRODUCT to NAME
                XmlNode product = item.SelectSingleNode("PRODUCT");
                XmlElement name = doc.CreateElement("NAME");
                name.InnerXml = product.InnerXml;
                item.ReplaceChild(name, product);

                // rename SHORTNOTE to SHORT_DESCRIPTION
                XmlNode shortNote = item.SelectSingleNode("SHORTNOTE");
                XmlElement shortDescription = doc.CreateElement("SHORT_DESCRIPTION");
                shortDescription.InnerXml = shortNote.InnerXml;
                item.ReplaceChild(shortDescription, shortNote);

                // transform CATEGORYTEXTs to CATEGORIES/CATEGORY
                XmlNodeList categorytexts = item.SelectNodes("CATEGORYTEXT");
                XmlElement cetegories = doc.CreateElement("CATEGORIES");
                foreach (XmlNode categorytext in categorytexts)
                {
                    XmlElement category = doc.CreateElement("CATEGORY");
                    string cattxt = categorytext.InnerXml;
                    category.InnerXml = cattxt.Substring(0, cattxt.IndexOf('\n') > 0 ? cattxt.IndexOf('\n') - 1 : cattxt.Length).Replace('|', '>');;
                    cetegories.AppendChild(category);
                    categorytext.ParentNode.RemoveChild(categorytext);
                }
                item.AppendChild(cetegories);

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

                // transform STOCK to AVAILABILITY
                XmlElement availability = doc.CreateElement("AVAILABILITY");
                if (stock.InnerText == "true")
                {
                    availability.InnerText = "Skladem";
                }
                else
                {
                    availability.InnerText = "Na dotaz";
                }
                item.AppendChild(availability);

                // transform STOCK and STOCKLEVEL to STOCK/AMOUNT
                XmlNode stockLevel = item.SelectSingleNode("STOCKLEVEL");
                stock.InnerText = "";
                XmlElement amount = doc.CreateElement("AMOUNT");
                amount.InnerXml = Regex.Match(stockLevel.InnerXml, @"\d+").Value;
                stock.AppendChild(amount);
                item.AppendChild(stock);
                item.RemoveChild(stockLevel);
            }
        }
    }
}
