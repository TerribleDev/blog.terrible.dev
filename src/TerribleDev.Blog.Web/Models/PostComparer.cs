using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TerribleDev.Blog.Web.Models
{
    public class PostComparer
    {
        public static PostComparisonByDateInternal PostComparisonByDate = new PostComparisonByDateInternal();

        public class PostComparisonByDateInternal : IComparer<IPost>
        {
            public int Compare([AllowNull] IPost x, [AllowNull] IPost y)
            {
                return DateTime.Compare(x.PublishDate, y.PublishDate);
            }
        }
    }
}
                    