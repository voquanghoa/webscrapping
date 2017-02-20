using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DwList
{
    [DataContract]
    public class MultiChoiceQuestion
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "is_correct")]
        public bool IsCorrect { get; set; }
    }

    [DataContract]
    public class MultiChoice: BaseLesson
    {
        [DataMember(Name = "questions")]
        public List<MultiChoiceQuestion> Questions { get; set; }
        public MultiChoice(): base("multi_choice")
        {

        }
    }
}
