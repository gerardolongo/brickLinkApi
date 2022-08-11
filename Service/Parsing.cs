using System;
using brickLinkApi.Domain.Model;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using HtmlAgilityPack;

namespace brickLinkApi.Service
{
    public class Parsing
    {
        private string numItem;

        public Parsing(string numItem)
        {
            this.numItem = numItem;
        }

        public string parseHTML()
        {
            string url = "https://www.bricklink.com/catalogItemInv.asp?S=" + this.numItem;
            var response = CallUrl(url).Result;
            var brickLinkModel = ParseHtml(response);
            return WriteToString(brickLinkModel);
        }

        private async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

        private Inventory ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var trTables = htmlDoc.DocumentNode.Descendants("tr")
                    .Where(node => node.GetAttributeValue("class", "").Contains("IV_ITEM"))
                    .Where(tr => tr.Elements("td").Count() > 1)
                    .ToList();

            Inventory inventory = new Inventory()
            {
                Items = new List<Item>()
            };

            foreach (var link in trTables)
            {
                Item item = new Item();

                var numItem = link.ChildNodes.Skip(1).Take(1).ToList();
                var items = link.ChildNodes.Skip(2).Take(1).ToList();
                var href = items[0].ChildNodes.Where(node => node.InnerHtml != "&nbsp;").ToList();

                item.ItemID = href[0].InnerHtml;
                var hrefAttribute = href[0].GetAttributeValue("href", string.Empty);
                if (hrefAttribute.Contains("idColor"))
                {
                    var indexOf = hrefAttribute.LastIndexOf("=");
                    item.Color = hrefAttribute.Substring(indexOf + 1);
                    item.MinQty = Convert.ToInt32(numItem[0].InnerHtml.Replace("&nbsp;", ""));
                    item.ItemType = "P";
                    inventory.Items.Add(item);
                    removeDuplicate(inventory);
                }
            }

            return inventory;
        }

        private void removeDuplicate(Inventory model)
        {
            model.Items = model.Items.GroupBy(x => new { x.ItemID, x.Color })
                  .Select(x => new Item(x.First().ItemType, x.Key.ItemID, x.Key.Color, x.First().MaxPrice, x.Sum(x => x.MinQty)))
                  .ToList();

        }

        private string WriteToString(Inventory model)
        {
            XmlSerializer writer = new XmlSerializer(typeof(Inventory));
            using (StringWriter textWriter = new StringWriter())
            {
                writer.Serialize(textWriter, model);
                return textWriter.ToString();
            }
        }
    }
}

