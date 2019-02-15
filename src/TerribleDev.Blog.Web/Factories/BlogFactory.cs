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
using TerribleDev.Blog.Web.Factories;

namespace TerribleDev.Blog.Web
{
    public class BlogFactory
    {
        private HighlightFactory highlightFactory = new HighlightFactory();
        public List<IPost> GetAllPosts(string domain)
        {
            // why didn't I use f# I'd have a pipe operator by now
            var posts = GetPosts();
            var postsAsText = posts.Select(GetFileText);
            return Task.WhenAll(postsAsText).Result.Select(b => ParsePost(b.text, b.fileInfo.Name, domain)).ToList();
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
        public IPost ParsePost(string postText, string fileName, string domain)
        {
            var splitFile = postText.Split("---");
            var ymlRaw = splitFile[0];
            var markdownText = string.Join("", splitFile.Skip(1));
            var postSettings = ParseYaml(ymlRaw);
            var resolvedUrl = !string.IsNullOrWhiteSpace(postSettings.permalink) ? postSettings.permalink : fileName.Split('.')[0].Replace(' ', '-').WithoutSpecialCharacters();
            var codeBlocks = new List<string>();
            List<string> postImages = new List<string>();
            var pipeline = new MarkdownPipelineBuilder()
                                .Use(new AbsoluteLinkConverter(resolvedUrl, domain))
                                .Use<ImageRecorder>(new ImageRecorder(ref postImages))
                                .Use<TargetLinkExtension>()
                                .Use<PictureInline>()
                                .Use(new CodeRecorder(ref codeBlocks))
                                .UseMediaLinks()
                                .UseEmojiAndSmiley()
                                .Build();
            var postContent = Markdown.ToHtml(markdownText, pipeline);
            var postContentHighlighted = highlightFactory.Highlight(postContent);
            var postContentPlain = String.Join("", Markdown.ToPlainText(markdownText, pipeline).Split("<!-- more -->"));
            
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
                Images = postImages.Distinct().ToList(),
                CodeBlockLangs = codeBlocks
            };
        }
    }
}
