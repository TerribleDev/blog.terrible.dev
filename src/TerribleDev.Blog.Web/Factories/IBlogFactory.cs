using System.Collections.Generic;
using System.Threading.Tasks;
using TerribleDev.Blog.Web.Models;

namespace TerribleDev.Blog.Web
{
    public interface IBlogFactory
    {
        Task<IEnumerable<IPost>> GetAllPosts();
        IEnumerable<string> GetPosts();
        IPost ParsePost(string postText, string fileName);
        IPostSettings ParseYaml(string ymlText);
    }
}