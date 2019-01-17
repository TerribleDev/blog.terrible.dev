using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using TerribleDev.Blog.Web.Models;
using YamlDotNet.Serialization;
using Microsoft.AspNetCore.Html;
using TerribleDev.Blog.Web.Extensions;

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
        public IEnumerable<string> GetPosts() => Directory.EnumerateFiles("Posts", "*.md", SearchOption.TopDirectoryOnly);

        public PostSettings ParseYaml(string ymlText)
        {
            var serializer = new DeserializerBuilder().Build();
            return serializer.Deserialize<PostSettings>(ymlText);

        }
        public IPost ParsePost(string postText, string fileName)
        {
            Console.WriteLine(fileName);
            var splitFile = postText.Split("---");
            var ymlRaw = splitFile[0];
            var postContent = Markdig.Markdown.ToHtml(string.Join("", splitFile.Skip(1)));
            var postSettings = ParseYaml(ymlRaw);
            var resolvedUrl = !string.IsNullOrWhiteSpace(postSettings.permalink) ? postSettings.permalink : fileName.Split('.')[0].Replace(' ', '-').WithoutSpecialCharacters();
            return new Post()
            {
                PublishDate = postSettings.date,
                tags = postSettings.tags,
                Title = postSettings.title,
                Url = resolvedUrl,
                Content = new HtmlString(postContent)
            };
        }

    }
}
