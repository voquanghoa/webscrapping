using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DwList
{
    [DataContract]
    public class BaseLesson
    {
        [DataMember(Name = "type")]
        public string LessonType { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        protected BaseLesson(string type)
        {
            this.LessonType = type;
        }
    }
}
