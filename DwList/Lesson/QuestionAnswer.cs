using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DwList
{
    [DataContract]
    public class QuestionAnswerItem
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "answer")]
        public string Answer { get; set; }
    }

    [DataContract]
    public class QuestionAnswer : BaseLesson
    {
        [DataMember(Name = "questions")]
        public List<QuestionAnswerItem> Questions { get; set; }

        public QuestionAnswer() : base("question_answer") { }
    }
}
