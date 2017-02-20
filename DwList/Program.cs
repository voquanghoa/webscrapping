using DwList.LessonParser;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DwList
{
    class Program
    {
        private static string localDataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        private static List<string> ignoredClass = new List<string>()
        {
            "modular assign_image"
        };

        static void Main(string[] args)
        {
            try
            {
                CreateFolder(localDataPath);
                var url = "http://www.dw.com/de/viele-k%C3%B6che-verderben-den-brei/l-36385982";
                var xpath = "//div[@id=\"bodyContent\"]//ul[@class=\"smallList\"][1]/li/a";

                HtmlNode.ElementsFlags.Remove("form");
                HtmlNode.ElementsFlags.Remove("p");
                HtmlNode.ElementsFlags.Remove("option");

                var html = new HtmlWeb().Load(url);
                var nodes = html.DocumentNode.SelectNodes(xpath).Where(x => x.Attributes.Count == 1).ToArray();
                var links = nodes.Select(x => new Href(x)).ToArray();
                foreach (var link in links)
                {
                    Console.WriteLine($"{link.Text} -- {link.Url}");
                    loadPage(link);
                }
                Console.WriteLine("Download done. Press any key to finish.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Some errors occurs. Press any key to finish.");
                
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
            
            var dataPath = Path.Combine(localDataPath, href.Slug);
            if (File.Exists(Path.Combine(dataPath, "Done")))
            {
                return;
            }

            CreateFolder(dataPath);

            var html = new HtmlWeb().Load("http://www.dw.com" + href.Url);
            var document = html.DocumentNode;

            downloadTestContent(href, document, dataPath);
            downloadMp3(document, dataPath);
            downloadImage(document, dataPath);

            File.Create(Path.Combine(dataPath, "Done"));
        }

        private static void downloadTestContent(Href href, HtmlNode node, string dataPath)
        {
            var formNodes = node.SelectNodes("//div[@class=\"dkTaskWrapper tab2\"]/form");
            var path = Path.Combine(dataPath, "lesson.json");
            
            var lesson = new Lesson();
            lesson.Title = href.Text;
            lesson.Lessons = new List<BaseLesson>();
            lesson.Content = TestUtils.ReformatContent(node.SelectSingleNode("//div[@class=\"dkTaskWrapper tab3\"]").InnerText);
            
            foreach (var form in formNodes)
            {
                BaseLesson baseLesson = null;
                var classes = form.Attributes["class"].Value;

                if(string.Equals(classes, "modular gaps"))
                {
                    baseLesson = CompleteTextParser.Parse(form);
                }

                if(string.Equals(classes, "modular test"))
                {
                    baseLesson = SingleChoiceParser.Parse(form);
                }

                if (string.Equals(classes, "modular assign"))
                {
                    baseLesson = QuestionAnswerParser.Parse(form);
                }

                if (baseLesson != null)
                {
                    lesson.Lessons.Add(baseLesson);
                }
                else
                {
                    if (!ignoredClass.Contains(classes))
                    {
                        throw new Exception($"Unsupport tag {classes} for page");
                    }
                }
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(lesson));
        }

        private static void downloadMp3(HtmlNode node, string dataPath)
        {
            var audioXpath = "//div[@class=\"mediaItem\"]/input[@name=\"file_name\"]";
            var audio = node.SelectSingleNode(audioXpath).Attributes["value"].Value;
            rawDownload(audio, Path.Combine(dataPath, "audio.mp3"));
        }

        private static void downloadImage(HtmlNode node, string dataPath)
        {
            var imgXpath = "//div[@class=\"mediaItem\"]/input[@name=\"preview_image\"]";
            var img = "http://www.dw.com" + node.SelectSingleNode(imgXpath).Attributes["value"].Value;
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
