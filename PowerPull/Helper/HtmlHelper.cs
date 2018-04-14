using System.Text.RegularExpressions;

namespace PowerPull.Helper
{
    public class HtmlHelper
    {
        public static string RemoveHtml(string text)
        { 
            return Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
        }
    }
}
