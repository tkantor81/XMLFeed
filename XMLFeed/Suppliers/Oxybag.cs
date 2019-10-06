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
                
                XmlNode price = item.SelectSingleNode("PRICE");
                if (price != null && price.InnerText == "?")
                {
                    // remove node <PRICE>?</PRICE>
                    item.RemoveChild(price);
                }
                else
                {
                    // rename PRICE to PURCHASE_PRICE
                    XmlElement purchasePrice = doc.CreateElement("PURCHASE_PRICE");
                    purchasePrice.InnerXml = price.InnerXml;
                    item.ReplaceChild(purchasePrice, price);
                }

                // rename PRICE_VAT to VAT
                XmlNode priceVat = item.SelectSingleNode("PRICE_VAT");
                if (priceVat != null)
                {
                    XmlElement vat = doc.CreateElement("VAT");
                    vat.InnerXml = priceVat.InnerXml;
                    item.ReplaceChild(vat, priceVat);
                }

                // rename DMOC_VC_DPH to PRICE_VAT
                XmlNode dmocVcDPH = item.SelectSingleNode("DMOC_VC_DPH");
                if (dmocVcDPH != null)
                {
                    priceVat = doc.CreateElement("PRICE_VAT");
                    priceVat.InnerXml = dmocVcDPH.InnerXml;
                    item.ReplaceChild(priceVat, dmocVcDPH);
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
                XmlNode ean = item.SelectSingleNode("EAN");
                if (ean.InnerXml == "")
                {
                    ean.InnerXml = "0";
                }
            }
        }
    }
}
