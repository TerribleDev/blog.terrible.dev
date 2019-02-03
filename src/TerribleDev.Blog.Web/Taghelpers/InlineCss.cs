using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Threading.Tasks;

namespace TerribleDev.Blog.Web.Taghelpers
{
    [HtmlTargetElement("inline-style")]
    public class InlineStyleTagHelper : TagHelper
    {
        [HtmlAttributeName("href")]
        public string Href { get; set; }

        private IHostingEnvironment HostingEnvironment { get; }
        private IMemoryCache Cache { get; }

 

        public InlineStyleTagHelper(IHostingEnvironment hostingEnvironment, IMemoryCache cache)
        {
            HostingEnvironment = hostingEnvironment;
            Cache = cache;
        }

 
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var path = Href;

            // Get the value from the cache, or compute the value and add it to the cache
            var fileContent = await Cache.GetOrCreateAsync("InlineStyleTagHelper-" + path, async entry =>
            {
                var fileProvider = HostingEnvironment.WebRootFileProvider;
                if(HostingEnvironment.IsDevelopment())
                {
                    var changeToken = fileProvider.Watch(path);
                    entry.AddExpirationToken(changeToken);
                }

                entry.SetPriority(CacheItemPriority.NeverRemove);

                var file = fileProvider.GetFileInfo(path);
                if (file == null || !file.Exists)
                    return null;

                return await ReadFileContent(file);
            });

            if (fileContent == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "style";
            output.Attributes.RemoveAll("href");
            output.Content.AppendHtml(fileContent);
        }

        private static async Task<string> ReadFileContent(IFileInfo file)
        {
            using (var stream = file.CreateReadStream())
            using (var textReader = new StreamReader(stream))
            {
                return await textReader.ReadToEndAsync();
            }
        }
    }
}