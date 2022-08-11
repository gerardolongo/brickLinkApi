using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace brickLinkApi.Domain.Model
{
    [XmlRoot(ElementName = "INVENTORY")]
    public class Inventory
    {
        [XmlElement(ElementName = "ITEM")]
        public List<Item> Items { get; set; }
    }
    public class Item
    {
        public Item(string ItemType, string ItemID, string Color, int MaxPrice, int MinQty)
        {
            this.ItemType = ItemType;
            this.ItemID = ItemID;
            this.Color = Color;
            this.MaxPrice = MaxPrice;
            this.MinQty = MinQty;
        }

        public Item()
        {

        }

        [XmlElement(ElementName = "ITEMTYPE")]
        public string ItemType { get; set; }
        [XmlElement(ElementName = "ITEMID")]
        public string ItemID { get; set; }
        [XmlElement(ElementName = "COLOR")]
        public string Color { get; set; }
        [XmlElement(ElementName = "MAXPRICE")]
        public int MaxPrice { get; set; }
        [XmlElement(ElementName = "MINQTY")]
        public int MinQty { get; set; }
    }
}

