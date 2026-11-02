using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Extrae los &lt;keyword&gt;...&lt;/keyword&gt; de un texto y devuelve el texto limpio.
/// </summary>
public static class KeywordTagParser
{
    public static List<string> Extract(string input, out string cleanText)
    {
        List<string> keywords = new List<string>();
        MatchCollection matches = Regex.Matches(input, @"<keyword>(.*?)</keyword>");

        foreach (Match match in matches)
        {
            if (match.Success)
            {
                keywords.Add(match.Groups[1].Value);
            }
        }

        cleanText = Regex.Replace(input, @"</?keyword>", "");
        return keywords;
    }
}
