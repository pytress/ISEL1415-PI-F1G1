using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class WWOParser : IParser<Dictionary<string, string>>
    {
        const char DELIMITER = '=';

        Dictionary<string, string> Parse(string[] args)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            foreach (string arg in args)
            {
                string[] keyValue = arg.Split(DELIMITER);
                if (keyValue.Length >= 2)                           // TODO: what to do when we get more than 1 equal (=)?
                    keyValuePairs.Add(keyValue[0], keyValue[1]);
            }

            return keyValuePairs;
        }
    }
}
