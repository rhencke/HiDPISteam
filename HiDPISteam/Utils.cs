using System;

namespace ConsoleApplication1
{
    static internal class Utils
    {
        public static string Unquote(string text)
        {
            if (String.IsNullOrEmpty(text))
                return text;
            if (text.Length >= 2 && text.StartsWith("\"") && text.EndsWith("\""))
            {
                return text.Substring(1, text.Length - 2);
            }
            return text;
        }

        public static string Quote(string value)
        {
            return "\"" + value + "\"";
        }
    }
}