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

            completeText.Title = node.SelectSingleNode(node.XPath + "/div[@class=\"news pseudo\"]").InnerText;
            completeText.Context = node.SelectSingleNode(node.XPath + "/p[@class=\"dkLargeText\"]").InnerHtml.Replace("<input correct=\"", "{{").Replace("\" type=\"text\" class=\"lineLeft\">", "}}");
            completeText.Words = new List<string>();
            var begin = 0;
            var end =0;

            while((begin = completeText.Context.IndexOf("{{", begin)) >= 0)
            {
                end = completeText.Context.IndexOf("}}", begin);
                completeText.Words.Add(completeText.Context.Substring(begin + 2, end - begin - 2));
                begin = end + 2;
            }

            completeText.Words = completeText.Words.OrderBy(x => Guid.NewGuid()).ToList();

            return completeText;
        }
    }
}
