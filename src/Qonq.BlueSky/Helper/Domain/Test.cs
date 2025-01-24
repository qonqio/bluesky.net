using System;
using System.Collections.Generic;
using System.Text;

namespace Qonq.BlueSky.Helper.Domain
{
    public static class Test
    {
        private static List<string> TLDs = new List<string> { ".com", ".org", ".net", ".nl" };

        public static bool IsValidDomain(string str)
        {
            return TLDs.Exists(tld =>
            {
                int i = str.LastIndexOf(tld);
                if (i == -1)
                {
                    return false;
                }
                return str[i - 1] == '.' && i == str.Length - tld.Length;
            });
        }
    }
}
