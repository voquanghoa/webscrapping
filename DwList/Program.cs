using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DwList
{
    class Href
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public Href(HtmlNode node)
        {
            Text = node.InnerText;
            Url = node.Attributes["href"].Value;
        }
    }

    class Program
    {
        private static string localDataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");

        static void Main(string[] args)
        {
            try
            {
                CreateFolder(localDataPath);
                var url = "http://www.dw.com/de/viele-k%C3%B6che-verderben-den-brei/l-36385982";
                var xpath = "//div[@id=\"bodyContent\"]//ul[@class=\"smallList\"][1]/li/a";
                var html = new HtmlWeb().Load(url);
                var nodes = html.DocumentNode.SelectNodes(xpath).Where(x => x.Attributes.Count == 1).ToArray();
                var links = nodes.Select(x => new Href(x)).ToArray();
                foreach (var link in links)
                {
                    Console.WriteLine($"{link.Text} -- {link.Url}");
                    loadPage(link);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                
            }
            Console.ReadKey();
        }

        private static void CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
        
        private static void loadPage(Href href)
        {
            var dataPath = Path.Combine(localDataPath, href.Text);
            CreateFolder(dataPath);

            var textXpath = "//div[@class=\"dkTaskWrapper tab3\"]";
            var audioXpath = "//div[@class=\"mediaItem\"]/input[@name=\"file_name\"]";
            var imgXpath = "//div[@class=\"mediaItem\"]/input[@name=\"preview_image\"]";
            
            var html = new HtmlWeb().Load("http://www.dw.com" + href.Url);
            var document = html.DocumentNode;

            var text = document.SelectSingleNode(textXpath).InnerText;
            var audio = document.SelectSingleNode(audioXpath).Attributes["value"].Value;
            var img = "http://www.dw.com" + document.SelectSingleNode(imgXpath).Attributes["value"].Value;

            File.WriteAllText(Path.Combine(dataPath, "text.txt"), text);
            rawDownload(audio, Path.Combine(dataPath, "audio.mp3"));
            rawDownload(img, Path.Combine(dataPath, "photo.jpg"));
        }

        private static void rawDownload(string url, string fileName)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, fileName);
            }
        }
    }
}
