using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab3
{
    internal static class StringExtension
    {
        public static string ReplaceNthOccurrence(this string obj, string find, string replace, int nthOccurrence)
        {
            if (nthOccurrence > 0)
            {
                MatchCollection matchCollection = Regex.Matches(obj, Regex.Escape(find));
                if (matchCollection.Count >= nthOccurrence)
                {
                    Match match = matchCollection[nthOccurrence - 1];
                    return obj.Remove(match.Index, match.Length).Insert(match.Index, replace);
                }
            }
            return obj;
        }
    }
}
