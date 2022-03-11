using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerribleDev.Blog.Web.Factories
{
    public class CodeFactory
    {
        public async static Task<(string result, bool hasCode)> ReplaceFencedCode(string markdown)
        {
            
            // regex grab all text between backticks
            var regex = new Regex(@"```(.*?)```", RegexOptions.Singleline);
            var matches = regex.Matches(markdown);
            var result = await Task.WhenAll(matches.Select(async match =>
            {
                var code = match.Value;
                var codeContent = await new HttpClient().PostAsync("https://prismasaservice.azurewebsites.net/api/HttpTrigger", new StringContent(code));
                return (code, await codeContent.Content.ReadAsStringAsync());
            }));
            foreach(var (match, newValue) in result)
            {
                markdown = markdown.Replace(match, newValue);
            }
            return (markdown, matches.Count > 0);

        }
    }
}