using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DwList.LessonParser
{
    public class CompleteTextParser
    {
        public static CompleteText Parse(HtmlNode node)
        { 
            CompleteText completeText = new CompleteText();

            completeText.Title = TestUtils.ReformatContent(node.SelectSingleNode(node.XPath + "/div[@class=\"news pseudo\"]").InnerText);

            var contentNode = node.SelectSingleNode(node.XPath + "//p[@class=\"dkLargeText\"]");

            if (contentNode != null)
            {
                var rawText = contentNode
                    .InnerHtml.Replace("<input correct=\"", "{{")
                    .Replace("\" type=\"text\" class=\"lineLeft\">", "}}");

                completeText.Context = TestUtils.ReformatContent(rawText);

                completeText.Words = new List<string>();

                var begin = 0;
                var end = 0;

                while ((begin = completeText.Context.IndexOf("{{", begin)) >= 0)
                {
                    end = completeText.Context.IndexOf("}}", begin);
                    completeText.Words.Add(TestUtils.ReformatContent(completeText.Context.Substring(begin + 2, end - begin - 2)));
                    begin = end + 2;
                }

                completeText.Words = completeText.Words.OrderBy(x => Guid.NewGuid()).ToList();
            }
            else
            {
                foreach (var pNode in node.SelectNodes(node.XPath + "/div[contains(@class,\"dkWrapSegment\")]/p[@class=\"task\"]"))
                {
                    var code = pNode.InnerHtml.Replace("<span style=\"display:none;\"></span>", string.Empty);

                    var begin = code.IndexOf("<select");
                    if (begin >= 0)
                    {
                        var end = code.IndexOf("</select>", begin);
                        var selectNode = code.Substring(begin, end - begin + 9);

                        var correct = TestUtils.SelectSingleString(selectNode, "<select correct=\"([\\w\\s]+)\">");
                        var words = TestUtils.SelectStrings(selectNode, "<option value=\"[\\w\\s]+\">([\\w\\s]+)</option>").ToList();
                        if (!words.Contains(correct))
                        {
                            throw new Exception($"Array {string.Join(",", words.ToArray())} does not contains {correct}");
                        }

                        var finalCode = $"[[{correct}|{string.Join("|", words)}]]";
                        completeText.Context += code.Replace(selectNode, finalCode) + "</br>";
                    }
                    else
                    {
                        var pattern = "<input correct=\"([\\s\\w]+)\" type=\"text\" class=\"lineLeft\">";

                        var regex = Regex.Match(code, pattern);
                        if (regex.Success)
                        {
                            var word = regex.Groups[1].Value;
                            completeText.Context += code.Replace(regex.Groups[0].Value, "{{" + word + "}}") + "</br>";
                        }else
                        {
                            throw new Exception("Sai sai");
                        }
                    }
                }
            }

            

            return completeText;
        }
    }
}
