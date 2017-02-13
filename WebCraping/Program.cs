using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCraping
{
    class Program
    {
        static Dictionary<string, string> tokenReplaces = new Dictionary<string, string>()        {
            { "&nbsp;"," " },
            { "&#8217;", "'"}
        };
        static void Main(string[] args)
        {
            var html = new HtmlWeb().Load("http://slowgerman.com/inhaltsverzeichnis/");
     

            var nodes = html.DocumentNode.SelectNodes("//ul[@class=\"lcp_catlist\"]/li/a").ToArray();
            foreach (HtmlNode item in nodes)
            {
                Console.WriteLine(item.Attributes["title"].Value + "  " + item.Attributes["href"].Value);
                DownloadPage(item.Attributes["title"].Value, item.Attributes["href"].Value);
            }

            Console.ReadKey();
        }

        private static string readTitle(string origin)
        {
            return origin.Substring(origin.LastIndexOf(":")+1).Trim();
        }

        private static string ReformatContent(string content)
        {
            foreach (var key in tokenReplaces)
            {
                content = content.Replace(key.Key, key.Value);
            }

            if (content.Contains("&#"))
            {
                throw new Exception("Sai sai");
            }

            return content;
        }

        private static void DownloadPage(string saveTo, string url)
        {
            var html = new HtmlWeb().Load(url);
            var contentNodes = html.DocumentNode.SelectNodes("//div[@class=\"entry-content\"]/p");
            var content = new StringBuilder();
            foreach (var node in contentNodes)
            {
                content.Append(ReformatContent(node.InnerText));
                content.Append("\n\n");
            }

            var str = content.ToString();
        }
    }
}
