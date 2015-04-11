using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Ficha1
{
    class WWOClient
    {
        const string WWOURL = "http://api.worldweatheronline.com/free/v2/past-weather.ashx";
        const string[] validKeys = { "local", "start", "enddate" };

        private string localValue;
        private string startDateValue;
        private string endDateValue;

        public WWOClient(IDictionary<string, string> keyValueArgs)
        {
            localValue = keyValueArgs[validKeys[0]];
            startDateValue = keyValueArgs[validKeys[1]];
            endDateValue = keyValueArgs[validKeys[2]];
        }

        void RequestData()
        {
        }
    }
}

