using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DwList
{
    [DataContract]
    public class Lesson
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "lessons")]
        public List<BaseLesson> Lessons { get; set; }
    }
}
