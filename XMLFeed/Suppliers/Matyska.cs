using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace XMLFeed.Suppliers
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
                // remove all items from category "Lepidla, rámy a příslušenství"
                bool removeItem = false;
                XmlNodeList categorytexts = item.SelectNodes("CATEGORYTEXT");
                foreach (XmlNode categorytext in categorytexts)
                { 
                    if (categorytext.InnerXml.Contains("Lepidla, rámy a příslušenství"))
                    {
                        removeItem = true;
                        break;
                    }
                }
                if (removeItem)
                {
                    item.ParentNode.RemoveChild(item);
                    continue;
                }

                // rename ITEM_ID to CODE, add Prefix
                XmlNode itemId = item.SelectSingleNode("ITEM_ID");
                XmlElement code = doc.CreateElement("CODE");
                code.InnerXml = Prefix + itemId.InnerXml;
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
                    0         Skladem       1 - 3 dny
                    else      ?             Na dotaz
                */
                if (Int32.TryParse(deliveryDate.InnerXml, out int delivery))
                {
                    switch (delivery)
                    {
                        case 0:
                            availability.InnerXml = "1 - 3 dny";
                            break;
                        default:
                            availability.InnerXml = "Na dotaz";
                            break;
                    }
                }
                else
                {
                    availability.InnerXml = "Vyprodáno";
                }
                item.ReplaceChild(availability, deliveryDate);

                // rename LISTPRICE_VAT to STANDARD_PRICE
                XmlNode listpriceVat = item.SelectSingleNode("LISTPRICE_VAT");
                XmlElement stdPrice = doc.CreateElement("STANDARD_PRICE");
                if (Double.TryParse(listpriceVat.InnerXml, NumberStyles.Any, new CultureInfo("en-US"), out double price))
                {
                    stdPrice.InnerXml = Math.Round(price, 0, MidpointRounding.AwayFromZero).ToString();
                }
                else
                {
                    stdPrice.InnerXml = listpriceVat.InnerXml;
                }
                item.ReplaceChild(stdPrice, listpriceVat);

                // rename YOURPRICE_VAT to PURCHASE_PRICE
                XmlNode yourpriceVat = item.SelectSingleNode("YOURPRICE_VAT");
                XmlElement purchasePrice = doc.CreateElement("PURCHASE_PRICE");
                if (Double.TryParse(yourpriceVat.InnerXml, NumberStyles.Any, new CultureInfo("en-US"), out double price2))
                {
                    purchasePrice.InnerXml = Math.Round(price2, 0, MidpointRounding.AwayFromZero).ToString();
                }
                else
                {
                    purchasePrice.InnerXml = yourpriceVat.InnerXml;
                }
                item.ReplaceChild(purchasePrice, yourpriceVat);

                // get MANUFACTURER
                XmlNode manufacturer = item.SelectSingleNode("MANUFACTURER");

                // transform YOURPRICE to PRICE_VAT as LISTPRICE_VAT - 5%
                XmlNode yourprice = item.SelectSingleNode("YOURPRICE");
                XmlElement priceVat = doc.CreateElement("PRICE_VAT");
                if (manufacturer?.InnerXml != "ARS UNA" && manufacturer?.InnerXml != "WALKER"
                    && Double.TryParse(listpriceVat.InnerXml, NumberStyles.Any, new CultureInfo("en-US"), out double price3))
                {
                    // discount
                    priceVat.InnerXml = Math.Round(price3 * 0.95, 0, MidpointRounding.AwayFromZero).ToString();
                }
                else
                {
                    // no discount
                    int decIndex = listpriceVat.InnerXml.IndexOf('.');
                    priceVat.InnerXml = listpriceVat.InnerXml.Substring(0, decIndex == -1 ? listpriceVat.InnerXml.Length : decIndex);
                }
                item.ReplaceChild(priceVat, yourprice);
            }
        }

        public override void Extend()
        {
            XmlNodeList items = ext.SelectNodes("/ITEMLIST/ITEM");
            foreach (XmlNode item in items)
            {
                XmlNode shopitem = doc.SelectSingleNode($"/SHOP/SHOPITEM[CODE='{Prefix + item.FirstChild.InnerXml}']");
                if (shopitem != null)
                {
                    XmlElement stock = doc.CreateElement("STOCK");
                    XmlElement amount = doc.CreateElement("AMOUNT");
                    amount.InnerText = item.LastChild.InnerXml;
                    stock.AppendChild(amount);
                    shopitem.AppendChild(stock);
                }
            }
        }
    }
}
