using System;
using McMaster.Extensions.CommandLineUtils;
using System.IO;
using Pastel;
using System.Drawing;
using System.Linq;

namespace TerribleDev.Blog.CLI
{
    class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication() { 
                Name = "Tempo",
                Description = "A simple blog generator"
            };
            app.MakeSuggestionsInErrorMessage = true;
            app.HelpOption(inherited: true);
            app.OnExecute(() => {
                app.ShowHelp();
                return 1;
            });
            app.Command("post", b =>
            {
                b.MakeSuggestionsInErrorMessage = true;
                b.OnExecute(() => {
                    b.ShowHelp();
                    Environment.Exit(1);
                });
                b.Command("list", a => {
                    a.OnExecute(() => {
                        PostBuilder.ListPosts().ForEach(Console.WriteLine);
                        return 0;
                    });
                });
                b.Command("new", a =>
                {
                    var title = a.Argument("Title", "The title of the post");
                    a.OnExecute(() =>
                    {
                        var titleValue = title.Value;
                        var fileName = $"{titleValue.Replace(" ", "-")}.md";
                        var targetDir = Path.Combine("Posts", fileName);
                        var assetPathName = fileName.Replace(".md", "");
                        var assetPath = Path.Combine("wwwroot", "img", assetPathName);
                        if (!Directory.Exists("Posts"))
                        {
                            Console.Error.WriteLine(($"Cannot find post directory, are you sure you are in the blog directory?").Pastel("#ff3c2e"));
                            return 1;
                        }
                        Console.WriteLine(("Building file 🚀").Pastel("#80ff40"));
                        File.WriteAllText(Path.Combine("Posts", fileName), PostBuilder.Build(titleValue));
                        Console.WriteLine(("Creating wwwroot directory 🛠").Pastel("#80ff40"));
                        Directory.CreateDirectory(assetPath);
                        Console.WriteLine(("Adding keep files 📝").Pastel("#80ff40"));
                        File.Create(Path.Combine(assetPath, ".keep"));
                        Console.WriteLine(("Done! 🎉").Pastel("#80ff40"));
                        return 0;
                    });
                });
            });
            try {
                return app.Execute(args);
            } 
            catch (UnrecognizedCommandParsingException e) {
                Console.WriteLine();
                Console.Error.WriteLine(e.Message.Pastel("#ff3c2e"));
                Console.WriteLine();
                Console.Error.WriteLine($"The most similar command is {Environment.NewLine} {e.NearestMatches.FirstOrDefault()}");
                Console.WriteLine();
                return 1;
            }
            catch (CommandParsingException e) {
                Console.Error.WriteLine(e.Message.Pastel("#ff3c2e"));
                return 1;
            }
        }
    }
}
