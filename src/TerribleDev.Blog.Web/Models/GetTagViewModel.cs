using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerribleDev.Blog.Web.Models
{
    public class GetTagViewModel
    {
        public IEnumerable<IPost> Posts { get; set; }
        public string Tag { get; set; }
    }
}
