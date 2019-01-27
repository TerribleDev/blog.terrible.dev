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

namespace TerribleDev.Blog.Web
{
    public class BlogFactory
    {
        public List<IPost> GetAllPosts()
        {
            // why didn't I use f# I'd have a pipe operator by now
            var posts = GetPosts();
            var postsAsText = posts.Select(GetFileText);
            return Task.WhenAll(postsAsText).Result.AsParallel().Select(b => ParsePost(b.text, b.fileInfo.Name)).ToList();
        }

        private static async Task<(string text, FileInfo fileInfo)> GetFileText(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var text = await File.ReadAllTextAsync(fileInfo.FullName);
            return (text, fileInfo);

        }

        public IEnumerable<string> GetPosts() => Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), "Posts"), "*.md", SearchOption.TopDirectoryOnly);

        public PostSettings ParseYaml(string ymlText)
        {
            var serializer = new DeserializerBuilder().Build();
            return serializer.Deserialize<PostSettings>(ymlText);

        }
        public IPost ParsePost(string postText, string fileName)
        {
            var splitFile = postText.Split("---");
            var ymlRaw = splitFile[0];
            var markdownText = string.Join("", splitFile.Skip(1));
            List<string> postImages = new List<string>();
            var pipeline = new MarkdownPipelineBuilder()
                                .Use<TargetLinkExtension>()
                                .Use<ImageRecorder>(new ImageRecorder(postImages))
                                .UseMediaLinks()
                                .UseEmojiAndSmiley()
                                .Build();
            var postContent = Markdown.ToHtml(markdownText, pipeline);
            var postContentPlain = String.Join("", Markdown.ToPlainText(markdownText, pipeline).Split("<!-- more -->"));
            var postSettings = ParseYaml(ymlRaw);
            var resolvedUrl = !string.IsNullOrWhiteSpace(postSettings.permalink) ? postSettings.permalink : fileName.Split('.')[0].Replace(' ', '-').WithoutSpecialCharacters();
            var summary = postContent.Split("<!-- more -->")[0];
            var postSummaryPlain = postContentPlain.Split("<!-- more -->")[0];
            return new Post()
            {
                PublishDate = postSettings.date.ToUniversalTime(),
                tags = postSettings.tags?.Select(a => a.Replace(' ', '-').WithoutSpecialCharacters().ToLower()).ToList() ?? new List<string>(),
                Title = postSettings.title,
                Url = resolvedUrl,
                Content = new HtmlString(postContent),
                Summary = new HtmlString(summary),
                SummaryPlain = postSummaryPlain,
                SummaryPlainShort = (postContentPlain.Length <= 147 ? postContentPlain : postContentPlain.Substring(0, 146)) + "...",
                ContentPlain = postContentPlain,
                Images = postImages.Distinct().Select(a => a.StartsWith('/') ? a : $"/{resolvedUrl}/{a}").ToList()
            };
        }
    }
}
