/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: extrae las keywords marcadas en un texto y devuelve el texto limpio.
 */

using System.Collections.Generic;
using System.Text.RegularExpressions;

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
