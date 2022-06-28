using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerribleDev.Blog.Web.Factories
{
    public class CodeFactory
    {
        private HttpClient httpClient = new HttpClient();
        private static Boolean IsDisabled = !String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DISABLE_PRISMA"));
        public async Task<(string result, bool hasCode)> ReplaceFencedCode(string markdown)
        {
            if(CodeFactory.IsDisabled)
            {
                return (markdown, false);
            }
            
            // regex grab all text between backticks
            var regex = new Regex(@"```(.*?)```", RegexOptions.Singleline);
            var matches = regex.Matches(markdown);
            var result = await Task.WhenAll(matches.Select(async match =>
            {
                var code = match.Value;
                var codeContent = await httpClient.PostAsync("https://prismasaservice.azurewebsites.net/api/HttpTrigger", new StringContent(code));
                if(!codeContent.IsSuccessStatusCode)
                {
                    Console.Error.WriteLine("Error posting code to prisma");
                }
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