using System.Collections.Generic;
using System.Linq;

namespace NRig
{
    internal static class ExtensionMethods
    {
        public static bool EndsWith(this List<char> list, string toCheckFor)
        {
            if (list.Count() < toCheckFor.Length)
                return false;

            for (int i = 0; i < toCheckFor.Length; i++)
            {
                if (list[list.Count() - toCheckFor.Length + i] != toCheckFor[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
