using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DwList
{
    [DataContract]
    public class CompleteText : BaseLesson
    {
        [DataMember(Name = "words")]
        public List<string> Words { get; set; }

        [DataMember(Name = "context")]
        public string Context { get; set; }

        public CompleteText(): base("complete_text")
        {
            
        }
    }
}
