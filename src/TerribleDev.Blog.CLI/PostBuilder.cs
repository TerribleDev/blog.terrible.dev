using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Pastel;
namespace TerribleDev.Blog.CLI
{
    public class PostBuilder
    {
        public static string Build(string title)
        {
            return $@"title: {title}{Environment.NewLine}date: {DateTime.Now.ToString("yyyy-MM-dd hh:mm")}{Environment.NewLine}tags:{Environment.NewLine}---";
        }
        public static List<string> ListPosts() 
        {
            if (!Directory.Exists("Posts"))
            {
                Console.Error.WriteLine($"Cannot find post directory, are you sure you are in the blog directory?");
                Environment.Exit(1);
            }
            var posts = Directory.GetFiles("Posts", "*.md");
            return posts.Select(x => (Path.GetFileNameWithoutExtension(x).Replace('-', ' ').Pastel(Color.LightBlue))).ToList();
        }
    }
}