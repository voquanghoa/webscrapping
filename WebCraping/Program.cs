using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        static string fileLog = Path.Combine(Directory.GetCurrentDirectory(), "Log.txt");
        static string currentPath = Directory.GetCurrentDirectory();

        static void Log(string text)
        {
            File.AppendAllText(fileLog, text + "\n");
            Console.WriteLine(text);
        }

        static void Main(string[] args)
        {
            try
            {
                var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
                var test = ConfigurationManager.GetSection("test");
                CreateFolder(dataPath);

                loadCategory(dataPath, "http://slowgerman.com/inhaltsverzeichnis/");
                /*loadCategory(dataPath, "http://slowgerman.com/category/absolute-beginner/");
                loadCategory(dataPath, "http://slowgerman.com/category//alltag-2/");
                loadCategory(dataPath, "http://slowgerman.com/category/dialog-2/");
                loadCategory(dataPath, "http://slowgerman.com/category//essen-trinken/");
                loadCategory(dataPath, "http://slowgerman.com/category//feiertage/");
                loadCategory(dataPath, "http://slowgerman.com/category//freundschaft-familie/");
                loadCategory(dataPath, "http://slowgerman.com/category//geschichte/");
                loadCategory(dataPath, "http://slowgerman.com/category//gesundheit-soziales/");
                loadCategory(dataPath, "http://slowgerman.com/category//kunst-und-kultur/");
                loadCategory(dataPath, "http://slowgerman.com/category//sprache/");
                loadCategory(dataPath, "http://slowgerman.com/category//verkehr/");
                */
                
            }
            catch (Exception ex)
            {
                Log("Error " + ex.Message);
                Log(ex.StackTrace);
            }
        }

        private static void loadCategory(string baseDirectory, string url)
        {
            Log($"Load category url :::\t {url}");

            var folderName = Path.Combine(baseDirectory, GetUrlName(url));
            CreateFolder(folderName);

            var html = new HtmlWeb().Load(url);
            var nodes = html.DocumentNode.SelectNodes("//ul[@class=\"lcp_catlist\"]/li/a").ToArray();
            foreach (HtmlNode item in nodes)
            {
                Log(item.Attributes["title"].Value + "  " + item.Attributes["href"].Value);
                DownloadPage(folderName, item.Attributes["href"].Value);
            }

            Console.ReadKey();
        }

        private static string TinyPath(string path)
        {
            return path.Remove(0, currentPath.Length);
        }

        private static void DownloadPage(string saveTo, string url)
        {
            Log($"Load page ::: {url}");

            saveTo = Path.Combine(saveTo, GetUrlName(url));
            CreateFolder(saveTo);

            if (File.Exists(Path.Combine(saveTo, "done")))
            {
                Log("Skip " + TinyPath(saveTo));
                return;
            }

            try
            {
                var html = new HtmlWeb().Load(url);

                loadText(html.DocumentNode, saveTo);
                loadAudio(html.DocumentNode, saveTo);
                loadPhoto(html.DocumentNode, saveTo);

                File.Create(Path.Combine(saveTo, "done"));
            }
            catch(Exception ex)
            {
                Log("Error " + ex.Message);
                Log(ex.StackTrace);
            }
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
            Log($"Download {url} to {TinyPath(fileName)}");

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
                Log("Wrong content" + content);
                throw new Exception("Sai sai");
            }

            return content;
        }
    }
}
