using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwList.LessonParser
{
    public class SingleChoiceParser
    {
        public static SingleChoice Parse(HtmlNode rootNode)
        {
            var singleChoice = new SingleChoice();

            singleChoice.Title = TestUtils.ReformatContent(rootNode.SelectSingleNode(rootNode.XPath + "/div[@class=\"news pseudo\"]").InnerText);
            singleChoice.Questions = new List<SingleChoiceQuestion>();

            foreach (var node in rootNode.SelectNodes(rootNode.XPath + "/div[contains(@class, \"dkWrapSegment\")]"))
            {
                var singleChoiceQuestion = new SingleChoiceQuestion();
                singleChoiceQuestion.Title = TestUtils.ReformatContent(node.SelectSingleNode(node.XPath + "/h3").InnerText);
                singleChoiceQuestion.Answers = new List<string>();

                foreach (var answerNode in node.SelectNodes(node.XPath + "/div[@class=\"optionBox\"]/*"))
                {
                    if(string.Equals(answerNode.Name, "input"))
                    {
                        if(string.Equals(answerNode.Attributes["correct"].Value, "true"))
                        {
                            singleChoiceQuestion.CorrectIndex = singleChoiceQuestion.Answers.Count;
                        }
                    }

                    if (string.Equals(answerNode.Name, "label"))
                    {
                        singleChoiceQuestion.Answers.Add(TestUtils.ReformatContent(answerNode.InnerText));
                    }
                }

                singleChoice.Questions.Add(singleChoiceQuestion);
            }
            return singleChoice;
        }
    }
}
