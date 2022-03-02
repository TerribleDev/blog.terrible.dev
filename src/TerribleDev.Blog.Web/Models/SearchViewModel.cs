using System.Collections.Generic;
using System.Linq;

namespace TerribleDev.Blog.Web.Models
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }
        public IList<IPost> Posts { get;  set; }

    }
}