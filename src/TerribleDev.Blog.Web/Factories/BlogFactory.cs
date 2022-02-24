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
        public async Task<IEnumerable<IPost>> GetAllPostsAsync(string domain)
        {
            // why didn't I use f# I'd have a pipe operator by now
            var posts = GetPosts();
            return await Task.WhenAll(posts.Select(async (post) => {
                var (text, fileInfo) = await GetFileText(post);
                return ParsePost(text, fileInfo.Name, domain);
            }));
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
        public (string postContent, string postContentPlain, string summary, string postSummaryPlain, IList<string> postImages) ResolveContentForPost(string markdownText, string fileName, string resolvedUrl, string domain) {
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
            return (postContent, postContentPlain, summary, postSummaryPlain, postImages);
        }
        public IPost ParsePost(string postText, string fileName, string domain)
        {
            var splitFile = postText.Split("---");
            var ymlRaw = splitFile[0];
            var markdownText = string.Join("", splitFile.Skip(1));
            var postSettings = ParseYaml(ymlRaw);
            var resolvedUrl = !string.IsNullOrWhiteSpace(postSettings.permalink) ? postSettings.permalink : fileName.Split('.')[0].Replace(' ', '-').WithoutSpecialCharacters();

            return new Post()
            {
                PublishDate = postSettings.date.ToUniversalTime(),
                tags = postSettings.tags?.Select(a => a.Replace(' ', '-').WithoutSpecialCharacters().ToLower()).ToList() ?? new List<string>(),
                Title = postSettings.title,
                RelativeUrl = $"/{resolvedUrl}/",
                CanonicalUrl = $"https://blog.terrible.dev/{resolvedUrl}/",
                UrlWithoutPath = resolvedUrl,
                Content = new Lazy<IPostContent>(() => {
                    (string postContent, string postContentPlain, string summary, string postSummaryPlain, IList<string> postImages) = ResolveContentForPost(markdownText, fileName, resolvedUrl, domain);
                    return new PostContent() {
                        Content = new HtmlString(postContent),
                        Images = postImages,
                        ContentPlain = postContentPlain,
                        Summary = new HtmlString(summary),
                        SummaryPlain = postSummaryPlain,
                        SummaryPlainShort = (postContentPlain.Length <= 147 ? postContentPlain : postContentPlain.Substring(0, 146)) + "..." 
                    };
                }),
            };
        }
    }
}
