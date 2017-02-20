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

        public List<BaseLesson> Lessons { get; set; }
    }
}
