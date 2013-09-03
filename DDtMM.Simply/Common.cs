using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    static public class Common
    {
        static private readonly Regex stripCommentRegex = new Regex(@"/\*[^*]*\*+(?:[^/][^*]*\*+)*/");
        static private readonly Regex lineTerminatorRegex = new Regex(@"(\\\\)|\\(;)");
        /// <summary>
        /// Removes Comments in form of /* */ from string and replaces it with a space
        /// </summary>
        /// <param name="str">String to string</param>
        /// <returns>a String with all comments removed</returns>
        static public string StripComments(string str)
        {
            return stripCommentRegex.Replace(str, " ");
        }

        static public string UnescapeLineTerminators(string text)
        {
            return lineTerminatorRegex.Replace(text, "$1$2");
        }

    }
}
