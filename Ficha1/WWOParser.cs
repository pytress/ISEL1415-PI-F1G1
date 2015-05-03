using System.Collections.Generic;

namespace Ficha1
{
    class WWOParser : IParser<Dictionary<string, string>>
    {
        const char DELIMITER = '=';

        public Dictionary<string, string> Parse(string[] args)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            foreach (string arg in args)
            {
                string[] keyValue = arg.Split(DELIMITER);
                if (keyValue.Length >= 2) //TODO: what to do when we get none or more than 1 equal (=)? now is disregarding both cases
                    keyValuePairs.Add(keyValue[0], keyValue[1]);
            }

            return keyValuePairs;
        }
    }
}
