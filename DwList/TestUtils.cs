using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwList
{
    public class TestUtils
    {
        static Dictionary<string, string> tokenReplaces = new Dictionary<string, string>()        {
            { "&nbsp;"," " },
            { "&#8217;", "'"},
            { "&#8211;","-"},
            { "&#8230;", "..." }
        };

        public static string ReformatContent(string content)
        {
            foreach (var key in tokenReplaces)
            {
                content = content.Replace(key.Key, key.Value);
            }

            if (content.Contains("&#"))
            {
                throw new Exception("Sai sai");
            }

            return content;
        }
    }
}
