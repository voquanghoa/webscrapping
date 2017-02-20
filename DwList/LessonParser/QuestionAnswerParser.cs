using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwList.LessonParser
{
    public class QuestionAnswerParser
    {
        public static QuestionAnswer Parse(HtmlNode node)
        {
            var questionAnswer = new QuestionAnswer();

            questionAnswer.Title = TestUtils.ReformatContent(node.SelectSingleNode(
                node.XPath + "/div[@class=\"news pseudo\"]").InnerText);

            questionAnswer.Questions = new List<QuestionAnswerItem>();
            QuestionAnswerItem questionAnswerItem = null;

            foreach (var questionNode in node.SelectNodes(node.XPath + "/div[@class=\"dkWrapSegment\"]/*"))
            {
                if(string.Equals(questionNode.Name, "p"))
                {
                    questionAnswerItem = new QuestionAnswerItem()
                    {
                        Title = TestUtils.ReformatContent(questionNode.InnerText)
                    };
                }

                if (string.Equals(questionNode.Name, "input"))
                {
                    questionAnswerItem.Answer = TestUtils.ReformatContent(questionNode.Attributes["correct"].Value);
                    questionAnswer.Questions.Add(questionAnswerItem);
                    questionAnswerItem = null;
                }
            }

            return questionAnswer;
        }
    }
}
