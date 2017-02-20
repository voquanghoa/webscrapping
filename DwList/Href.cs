using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwList
{
    public class Href
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public Href(HtmlNode node)
        {
            Text = node.InnerText;
            Url = node.Attributes["href"].Value;
        }
    }
}
