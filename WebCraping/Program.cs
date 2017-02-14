using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebCraping
{
    class Program
    {
        static Dictionary<string, string> tokenReplaces = new Dictionary<string, string>()        {
            { "&nbsp;"," " },
            { "&#8217;", "'"},
            { "&#8211;","-"},
            { "&#8230;", "..." }
        };

        static void Main(string[] args)
        {
            var dataPath = Path.Combine(@"E:\DataDownload", "Data");
            CreateFolder(dataPath);

            loadCategory(dataPath, "http://slowgerman.com/inhaltsverzeichnis/");
        }

        private static void loadCategory(string baseDirectory, string url)
        {
            var folderName = Path.Combine(baseDirectory, GetUrlName(url));
            CreateFolder(folderName);

            var html = new HtmlWeb().Load(url);
            var nodes = html.DocumentNode.SelectNodes("//ul[@class=\"lcp_catlist\"]/li/a").ToArray();
            foreach (HtmlNode item in nodes)
            {
                Console.WriteLine(item.Attributes["title"].Value + "  " + item.Attributes["href"].Value);
                DownloadPage(folderName, item.Attributes["href"].Value);
            }

            Console.ReadKey();
        }

        private static void DownloadPage(string saveTo, string url)
        {
            saveTo = Path.Combine(saveTo, GetUrlName(url));
            CreateFolder(saveTo);

            if (File.Exists(Path.Combine(saveTo, "done")))
                return; 

            var html = new HtmlWeb().Load(url);

            loadText(html.DocumentNode, saveTo);
            loadAudio(html.DocumentNode, saveTo);
            loadPhoto(html.DocumentNode, saveTo);
        }

        private static void loadPhoto(HtmlNode htmlNode, string folder)
        {
            var fileName = Path.Combine(folder, "photo.jpg");
            if (!File.Exists(fileName))
            {
                var audioNode = htmlNode.SelectSingleNode(
                "//div[@id=\"main-content\"]//div[@class=\"entry-media\"]/img");
                rawDownload(audioNode.Attributes["src"].Value, fileName);
            }
        }

        private static void loadAudio(HtmlNode htmlNode, string folder)
        {
            var fileName = Path.Combine(folder, "audio.mp3");
            if (!File.Exists(fileName))
            {
                var audioNode = htmlNode.SelectSingleNode(
                "//div[@id=\"main-content\"]//div[@class=\"player-container\"]/audio/source");
                rawDownload(audioNode.Attributes["src"].Value, fileName);
            }
        }

        private static void loadText(HtmlNode htmlNode, string folder)
        {
            var fileName = Path.Combine(folder, "text.txt");
            if (!File.Exists(fileName))
            {
                var contentNodes = htmlNode.SelectNodes("//div[@class=\"entry-content\"]/p");
                var content = new StringBuilder();
                foreach (var node in contentNodes)
                {
                    content.Append(ReformatContent(node.InnerText));
                    content.Append("\n\n");
                }

                var str = content.ToString();

                File.WriteAllText(fileName, str);
            }
            
        }
        private static void rawDownload(string url, string fileName)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, fileName);
            }
        }

        private static string GetUrlName(string url)
        {
            url = url.Trim();
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            return url.Substring(url.LastIndexOf("/")+1);
        }

        private static void CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        private static string readTitle(string origin)
        {
            return origin.Substring(origin.LastIndexOf(":") + 1).Trim();
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
    }
}
