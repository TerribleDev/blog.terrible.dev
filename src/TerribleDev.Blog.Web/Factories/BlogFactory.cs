using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using TerribleDev.Blog.Web.Models;
using YamlDotNet.Serialization;
using Microsoft.AspNetCore.Html;
using Markdig;
using TerribleDev.Blog.Web.MarkExtension.TerribleDev.Blog.Web.ExternalLinkParser;
using TerribleDev.Blog.Web.MarkExtension;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace TerribleDev.Blog.Web
{
    public class BlogFactory
    {
        public IEnumerable<IPost> GetAllPosts(string domain)
        {
            // why didn't I use f# I'd have a pipe operator by now
            var posts = GetPosts();
            var list = new ConcurrentBag<IPost>();
            Parallel.ForEach(posts, post =>
            {
                var (text, fileInfo) = GetFileText(post);
                list.Add(ParsePost(text, fileInfo.Name, domain));
            });
            return list;
        }

        private static (string text, FileInfo fileInfo) GetFileText(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var text = File.ReadAllText(fileInfo.FullName);
            return (text, fileInfo);

        }

        public IEnumerable<string> GetPosts() => Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), "Posts"), "*.md", SearchOption.TopDirectoryOnly);

        public PostSettings ParseYaml(string ymlText)
        {
            var serializer = new DeserializerBuilder().Build();
            return serializer.Deserialize<PostSettings>(ymlText);

        }
        public IPost ParsePost(string postText, string fileName, string domain)
        {
            var splitFile = postText.Split("---");
            var ymlRaw = splitFile[0];
            var markdownText = string.Join("", splitFile.Skip(1));
            var postSettings = ParseYaml(ymlRaw);
            var resolvedUrl = !string.IsNullOrWhiteSpace(postSettings.permalink) ? postSettings.permalink : fileName.Split('.')[0].Replace(' ', '-').WithoutSpecialCharacters();
            List<string> postImages = new List<string>();
            var pipeline = new MarkdownPipelineBuilder()
                                .Use(new AbsoluteLinkConverter(resolvedUrl, domain))
                                .Use<ImageRecorder>(new ImageRecorder(ref postImages))
                                .Use<TargetLinkExtension>()
                                .UseMediaLinks()
                                .Use<PictureInline>()
                                .UseEmojiAndSmiley()
                                .Build();
            var postContent = Markdown.ToHtml(markdownText, pipeline);
            var postContentPlain = String.Join("", Markdown.ToPlainText(markdownText, pipeline).Split("<!-- more -->"));

            var summary = postContent.Split("<!-- more -->")[0];
            var postSummaryPlain = postContentPlain.Split("<!-- more -->")[0];

            return new Post()
            {
                PublishDate = postSettings.date.ToUniversalTime(),
                tags = postSettings.tags?.Select(a => a.Replace(' ', '-').WithoutSpecialCharacters().ToLower()).ToList() ?? new List<string>(),
                Title = postSettings.title,
                RelativeUrl = $"/{resolvedUrl}/",
                CanonicalUrl = $"https://blog.terrible.dev/{resolvedUrl}/",
                UrlWithoutPath = resolvedUrl,
                Content = new HtmlString(postContent),
                Summary = new HtmlString(summary),
                SummaryPlain = postSummaryPlain,
                SummaryPlainShort = (postContentPlain.Length <= 147 ? postContentPlain : postContentPlain.Substring(0, 146)) + "...",
                ContentPlain = postContentPlain,
                Images = postImages.Distinct().ToList()
            };
        }
    }
}
