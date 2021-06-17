using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TerribleDev.Blog.Web.Models;

namespace TerribleDev.Blog.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly PostCache postCache;
        public ApiController(PostCache postCache)
        {
            this.postCache = postCache;
        }
        [Route("all")]
        public ActionResult<IEnumerable<IPost>> PostCount(string key) 
        {
            if(string.Equals(key, "tommyisawesome")) 
            {
                return this.Ok(this.postCache.PostsAsLists);
            }
            return this.StatusCode(403);
        }
    }
}