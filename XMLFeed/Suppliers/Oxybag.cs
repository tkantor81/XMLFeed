using System;
using System.Collections.Generic;
using System.Globalization;
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
                // rename DMOC_VC_DPH to PRICE_VAT and limit according Minimum transformed item price
                XmlNode dmocVcDPH = item.SelectSingleNode("DMOC_VC_DPH");
                if (dmocVcDPH != null)
                {
                    if (Double.TryParse(dmocVcDPH.InnerXml, NumberStyles.Any, new CultureInfo("en-US"), out double parsedPrice))
                    {
                        if (parsedPrice < MinPrice)
                        {
                            item.ParentNode.RemoveChild(item);
                            continue;
                        }
                    }
                    else
                    {
                        item.ParentNode.RemoveChild(item);
                        continue;
                    }

                    XmlNode priceVat = doc.CreateElement("PRICE_VAT");
                    priceVat.InnerXml = dmocVcDPH.InnerXml;
                    item.ReplaceChild(priceVat, dmocVcDPH);
                }
                else
                {
                    item.ParentNode.RemoveChild(item);
                    continue;
                }

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

                // rename PRICE_VAT to VAT
                double dph = 0;
                XmlNode priceVatt = item.SelectSingleNode("PRICE_VAT");
                if (priceVatt != null)
                {
                    XmlElement vat = doc.CreateElement("VAT");
                    vat.InnerXml = priceVatt.InnerXml;
                    item.ReplaceChild(vat, priceVatt);
                    dph = Double.Parse(vat.InnerXml);
                }

                double total = 0;
                XmlNode price = item.SelectSingleNode("PRICE");
                if (price != null && price.InnerText == "?")
                {
                    // remove node <PRICE>?</PRICE>
                    item.RemoveChild(price);
                }
                else
                {
                    // rename PRICE to PURCHASE_PRICE and add VAT
                    if (Double.TryParse(price.InnerXml, NumberStyles.Any, new CultureInfo("en-US"), out double parsedPrice))
                    {
                        total = Math.Round(parsedPrice * (1 + (0.01 * dph)), 0, MidpointRounding.AwayFromZero);
                    }
                        
                    XmlElement purchasePrice = doc.CreateElement("PURCHASE_PRICE");
                    purchasePrice.InnerXml = total.ToString();
                    item.ReplaceChild(purchasePrice, price);
                }

                // transform PARAMs to TEXT_PROPERTIES/TEXT_PROPERTY
                XmlNode paramss = item.SelectSingleNode("PARAMS");
                if (paramss != null)
                {
                    XmlElement textProperties = doc.CreateElement("TEXT_PROPERTIES");
                    foreach (XmlNode param in paramss.ChildNodes)
                    {
                        XmlElement textProperty = doc.CreateElement("TEXT_PROPERTY");
                        XmlElement propName = doc.CreateElement("NAME");
                        propName.InnerText = param.FirstChild.InnerXml;
                        textProperty.AppendChild(propName);
                        XmlElement propValue = doc.CreateElement("VALUE");
                        propValue.InnerText = param.LastChild.InnerXml;
                        textProperty.AppendChild(propValue);
                        textProperties.AppendChild(textProperty);
                    }
                    item.AppendChild(textProperties);
                    item.RemoveChild(paramss);
                }

                // fill EAN if empty
                Transformation.FillEmptyEAN(item);
            }
        }
    }
}
