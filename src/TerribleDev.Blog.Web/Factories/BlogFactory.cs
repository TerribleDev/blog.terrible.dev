﻿using System;
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
using Schema.NET;
using System.Text.RegularExpressions;
using TerribleDev.Blog.Web.Factories;
using System.Text;
using System.Security.Cryptography;

namespace TerribleDev.Blog.Web
{
    public class BlogFactory
    {
        private CodeFactory _codeFactory = new CodeFactory();
        public async Task<IEnumerable<IPost>> GetAllPostsAsync(string domain)
        {
            // why didn't I use f# I'd have a pipe operator by now
            var posts = GetPosts();

            return await Task.WhenAll(posts.Select(async (post) =>
            {
                var (text, fileInfo) = await GetFileText(post);
                return await ParsePost(text, fileInfo.Name, domain);
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
        public async Task<(string postContent, string postContentPlain, string summary, string postSummaryPlain, IList<string> postImages, Boolean hasCode)> ResolveContentForPost(string markdownText, string fileName, string resolvedUrl, string domain)
        {
            List<string> postImages = new List<string>();
            var pipeline = new MarkdownPipelineBuilder()
                                .Use(new AbsoluteLinkConverter(resolvedUrl, domain))
                                .Use<ImageRecorder>(new ImageRecorder(ref postImages))
                                .Use<TargetLinkExtension>()
                                .UseMediaLinks()
                                .Use<PictureInline>()
                                .UseEmojiAndSmiley()
                                .Build();
            var (replacedText, hasCode) = await _codeFactory.ReplaceFencedCode(markdownText);
            var postContent = Markdown.ToHtml(replacedText, pipeline);
            var postContentPlain = String.Join("", Markdown.ToPlainText(replacedText, pipeline).Split("<!-- more -->"));
            var summary = postContent.Split("<!-- more -->")[0];
            var postSummaryPlain = postContentPlain.Split("<!-- more -->")[0];
            return (postContent, postContentPlain, summary, postSummaryPlain, postImages, hasCode);
        }
        public async Task<IPost> ParsePost(string postText, string fileName, string domain)
        {
            var splitFile = postText.Split("---");
            var ymlRaw = splitFile[0];
            var markdownText = string.Join("", splitFile.Skip(1));
            var postSettings = ParseYaml(ymlRaw);
            var resolvedUrl = !string.IsNullOrWhiteSpace(postSettings.permalink) ? postSettings.permalink : fileName.Split('.')[0].Replace(' ', '-').WithoutSpecialCharacters();
            var canonicalUrl = $"https://blog.terrible.dev/{resolvedUrl}/";
            return postSettings.isLanding ? await BuildLandingPage(fileName, domain, markdownText, postSettings, resolvedUrl, canonicalUrl) : await BuildPost(fileName, domain, markdownText, postSettings, resolvedUrl, canonicalUrl);
        }

        private async Task<Post> BuildPost(string fileName, string domain, string markdownText, PostSettings postSettings, string resolvedUrl, string canonicalUrl)
        {

            (string postContent, string postContentPlain, string summary, string postSummaryPlain, IList<string> postImages, bool hasCode) = await ResolveContentForPost(markdownText, fileName, resolvedUrl, domain);
            var ld = new Schema.NET.BlogPosting()
            {
                Headline = postSettings.title,
                DatePublished = postSettings.date,
                DateModified = postSettings.updated ?? postSettings.date,
                WordCount = postContentPlain.Split(' ').Length,
                ArticleBody = new Schema.NET.OneOrMany<string>(new HtmlString(postContentPlain).Value),
                Author = new Schema.NET.Person() { Name = "Tommy Parnell", AdditionalName = "TerribleDev", Url = new Uri("https://blog.terrible.dev/about") },
                Url = new Uri(canonicalUrl)
            };
            var breadcrumb = new Schema.NET.BreadcrumbList()
            {
                ItemListElement = new List<IListItem>() // Required
                        {
                            new ListItem() // Required
                            {
                                Position = 1, // Required
                                Url = new Uri("https://blog.terrible.dev/") // Required
                            },
                            new ListItem()
                            {
                                Position = 2,
                                Name = postSettings.title,
                            },
                        },
            };
            // regex remove picture and source tags but not the child elements
            var postContentClean = Regex.Replace(postContent, "<picture.*?>|</picture>|<source.*?>|</source>", "", RegexOptions.Singleline);
            var content = new PostContent()
            {
                Content = new HtmlString(postContent),
                Images = postImages,
                ContentPlain = postContentPlain,
                Summary = new HtmlString(summary),
                SummaryPlain = postSummaryPlain,
                SummaryPlainShort = (postContentPlain.Length <= 147 ? postContentPlain : postContentPlain.Substring(0, 146)) + "...",
                JsonLD = ld,
                JsonLDString = ld.ToHtmlEscapedString().Replace("https://schema.org", "https://schema.org/true"),
                JsonLDBreadcrumb = breadcrumb,
                JsonLDBreadcrumbString = breadcrumb.ToHtmlEscapedString().Replace("https://schema.org", "https://schema.org/true"),
                HasCode = hasCode,
                MarkdownMD5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(markdownText)).ToHexString()
            };
            var thumbNailUrl = string.IsNullOrWhiteSpace(postSettings.thumbnailImage) ?
             postImages?.FirstOrDefault() ?? "https://www.gravatar.com/avatar/333e3cea32cd17ff2007d131df336061?s=640"  : 
             $"{canonicalUrl}/{postSettings.thumbnailImage}";
            return new Post()
            {
                PublishDate = postSettings.date.ToUniversalTime(),
                UpdatedDate = postSettings.updated?.ToUniversalTime() ?? null,
                tags = postSettings.tags?.Select(a => a.Replace(' ', '-').WithoutSpecialCharacters().ToLower()).ToList() ?? new List<string>(),
                Title = postSettings.title,
                RelativeUrl = $"/{resolvedUrl}/",
                CanonicalUrl = canonicalUrl,
                UrlWithoutPath = resolvedUrl,
                isLanding = postSettings.isLanding,
                Content = content,
                ThumbnailImage = thumbNailUrl,
            };
        }
        private async Task<LandingPage> BuildLandingPage(string fileName, string domain, string markdownText, PostSettings postSettings, string resolvedUrl, string canonicalUrl)
        {
            (string postContent, string postContentPlain, string summary, string postSummaryPlain, IList<string> postImages, bool hasCode) = await ResolveContentForPost(markdownText, fileName, resolvedUrl, domain);
                    var breadcrumb = new Schema.NET.BreadcrumbList()
                    {
                        ItemListElement = new List<IListItem>() // Required
                        {
                            new ListItem() // Required
                            {
                                Position = 1, // Required
                                Url = new Uri("https://blog.terrible.dev/") // Required
                            },
                            new ListItem()
                            {
                                Position = 2,
                                Name = postSettings.title,
                            },
                        },
                    };
                    // regex remove picture and source tags but not the child elements
                    var content = new PostContent()
                    {
                        Content = new HtmlString(postContent),
                        Images = postImages,
                        ContentPlain = postContentPlain,
                        Summary = new HtmlString(summary),
                        SummaryPlain = postSummaryPlain,
                        SummaryPlainShort = (postContentPlain.Length <= 147 ? postContentPlain : postContentPlain.Substring(0, 146)) + "...",
                        JsonLDBreadcrumb = breadcrumb,
                        JsonLDBreadcrumbString = breadcrumb.ToHtmlEscapedString().Replace("https://schema.org", "https://schema.org/true"),
                        HasCode = hasCode,
                        MarkdownMD5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(markdownText)).ToHexString()
                    };
            return new LandingPage()
            {
                PublishDate = postSettings.date.ToUniversalTime(),
                UpdatedDate = postSettings.updated?.ToUniversalTime() ?? null,
                Title = postSettings.title,
                RelativeUrl = $"/{resolvedUrl}/",
                CanonicalUrl = canonicalUrl,
                UrlWithoutPath = resolvedUrl,
                isLanding = postSettings.isLanding,
                Content = content,
            };
        }
    }
}
