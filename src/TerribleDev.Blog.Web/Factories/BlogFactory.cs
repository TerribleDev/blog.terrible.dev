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
            var allPosts = posts.AsParallel().Select(a =>
            {
                var fileInfo = new FileInfo(a);
                var fileText = File.ReadAllText(fileInfo.FullName);
                return ParsePost(fileText, fileInfo.Name);
            });
            return allPosts.ToList();
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
                var pipeline = new MarkdownPipelineBuilder().Use<TargetLinkExtension>().UseMediaLinks().UseEmojiAndSmiley().Build();
                var postContent = Markdown.ToHtml(markdownText, pipeline);
                var postContentPlain = String.Join("", Markdown.ToPlainText(markdownText, pipeline).Split("<!-- more -->"));
                var postSettings = ParseYaml(ymlRaw);
                var resolvedUrl = !string.IsNullOrWhiteSpace(postSettings.permalink) ? postSettings.permalink : fileName.Split('.')[0].Replace(' ', '-').WithoutSpecialCharacters();
                var summary = postContent.Split("<!-- more -->")[0];
                var postSummaryPlain = postContentPlain.Split("<!-- more -->")[0];
                return new Post()
                {
                    PublishDate = postSettings.date,
                    tags = postSettings.tags?.Select(a => a.Replace(' ', '-').WithoutSpecialCharacters().ToLower()).ToList() ?? new List<string>(),
                    Title = postSettings.title,
                    Url = resolvedUrl,
                    Content = new HtmlString(postContent),
                    Summary = new HtmlString(summary),
                    SummaryPlain = postSummaryPlain,
                    ContentPlain = postContentPlain
                };
        }
    }
}
