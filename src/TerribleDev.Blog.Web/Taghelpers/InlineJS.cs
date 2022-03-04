using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TerribleDev.Blog.Web.Taghelpers
{
    [HtmlTargetElement("inline-script")]
    public class InlineScriptTagHelper : TagHelper
    {
        [HtmlAttributeName("src")]
        public string Src { get; set; }

        private IWebHostEnvironment HostingEnvironment { get; }
        private IMemoryCache Cache { get; }



        public InlineScriptTagHelper(IWebHostEnvironment hostingEnvironment, IMemoryCache cache)
        {
            HostingEnvironment = hostingEnvironment;
            Cache = cache;
        }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var paths = Src.Split(',');

            // Get the value from the cache, or compute the value and add it to the cache
            var fileContent = await Cache.GetOrCreateAsync("InlineScriptTagHelper-" + paths, async entry =>
            {
                var fileProvider = HostingEnvironment.WebRootFileProvider;
                var result = paths.Select(async path => {
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
                var allFinished = await Task.WhenAll(result);
                return string.Join("\n", allFinished);
            });

            if (fileContent == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "script";
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
