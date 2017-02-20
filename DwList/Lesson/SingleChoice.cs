using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DwList
{
    [DataContract]
    public class SingleChoice : BaseLesson
    {
        [DataMember(Name = "answers")]
        public List<string> Answers { get; set; }

        [DataMember(Name = "correct_index")]
        public int CorrectIndex { get; set; }

        public SingleChoice(): base("single_choice") { }
    }
}
