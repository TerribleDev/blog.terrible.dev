using System;
using System.Collections.Generic;
using System.Text;

namespace TerribleDev.Blog.MarkdownPlugins
{
        public static class StringExtension
        {
            public static string WithoutSpecialCharacters(this string str)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c == '-')
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
        }
}
