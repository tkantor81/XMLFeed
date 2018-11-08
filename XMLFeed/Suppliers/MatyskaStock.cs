using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XMLFeed.Suppliers
{
    public class MatyskaStock : Supplier
    {
        public MatyskaStock(Options opts) : base(opts)
        {
        }

        public override void Transform()
        {
            var transDoc = new XmlDocument();
            
            // transform ITEMLIST/ITEMs to SHOP/SHOPITEMs
            XmlElement shop = transDoc.CreateElement("SHOP");
           
            XmlNodeList items = doc.SelectNodes("/ITEMLIST/ITEM");
            foreach (XmlNode item in items)
            {
                XmlElement shopitem = transDoc.CreateElement("SHOPITEM");
                XmlElement code = transDoc.CreateElement("CODE");
                code.InnerText = item.FirstChild.InnerXml;
                shopitem.AppendChild(code);
                XmlElement stock = transDoc.CreateElement("STOCK");
                XmlElement amount = transDoc.CreateElement("AMOUNT");
                amount.InnerText = item.LastChild.InnerXml;
                stock.AppendChild(amount);
                shopitem.AppendChild(stock);
                shop.AppendChild(shopitem);
            }
            transDoc.AppendChild(shop);

            doc = transDoc;
        }
    }
}
