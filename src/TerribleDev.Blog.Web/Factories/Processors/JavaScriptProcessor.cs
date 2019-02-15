using System.Text.RegularExpressions;

namespace TerribleDev.Blog.Web.Factories.Processors
{
    public class JavaScriptProcessor
    {
        private Regex keywordRegex = new Regex(@"\b(in|of|if|for|while|finally|var|new|function|do|return|void|else|break|catch|instanceof|with|throw|case|default|try|this|switch|continue|typeof|delete|let|yield|const|export|super|debugger|as|async|await|static|import|from|as|)(?=[^\w])", RegexOptions.Compiled | RegexOptions.Multiline);
        private Regex literalRegex = new Regex(@"\b(true|false|null|undefined|NaN|Infinity)(?=[^\w])", RegexOptions.Compiled | RegexOptions.Multiline);
        private Regex[] quoteMarkRegexes = new Regex[]{ new Regex("(.*?)", RegexOptions.Compiled | RegexOptions.Multiline), new Regex(@"'(.*?)'", RegexOptions.Compiled | RegexOptions.Multiline)};
        public string Process(string input)
        {
            return input;
        }
    }
}