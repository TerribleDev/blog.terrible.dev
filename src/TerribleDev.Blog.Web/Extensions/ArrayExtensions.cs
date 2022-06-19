using System;

namespace TerribleDev.Blog.Web
{
    public static class ArrayExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            return Convert.ToHexString(bytes);
        }
    }
}