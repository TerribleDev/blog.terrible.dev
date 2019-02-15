using System.Text.RegularExpressions;

namespace TerribleDev.Blog.Web.Factories
{
    public class HighlightFactory
    {
        private Regex codeFenceLang = new Regex("(?=<code class=\"language-(.*?)\">(.*?)(?=</code>))", RegexOptions.Compiled | RegexOptions.Singleline);
        public string Highlight(string input)
        {
            return codeFenceLang.Replace(input, m => {
                return m.ToString();
            });
        }
    }
}