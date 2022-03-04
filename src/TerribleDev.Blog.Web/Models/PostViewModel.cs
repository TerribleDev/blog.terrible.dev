namespace TerribleDev.Blog.Web.Models
{
    public class PostViewModel
    {
        public IPost Post { get; set; }
        public bool IsAmp { get; set; } = false;

    }
}