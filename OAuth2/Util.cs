using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2
{
    internal static class Util
    {
        public static JsonReader ToJsonReader(this string @string)
        {
            return new JsonTextReader(new StringReader(@string));
        }
    }
}