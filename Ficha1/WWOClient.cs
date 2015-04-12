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
        const string SCHEMA = "http://";
        const string HOST = "api.worldweatheronline.com";
        const string WWO_API_PATH = "free/v2/past-weather.ashx";
        static readonly string[] validKeys = { "-local", "-start", "-enddate" };

        private string localValue;
        private string startDateValue;
        private string endDateValue;

        private RestClient rClient;
        private RestRequest rReq;

        public WWOClient(IDictionary<string, string> keyValueArgs)
        {
            localValue = keyValueArgs[validKeys[0]];
            startDateValue = keyValueArgs[validKeys[1]];
            endDateValue = keyValueArgs[validKeys[2]];

            rClient = new RestClient(SCHEMA + HOST);
        }

        public void RequestData()
        {
            rReq = new RestRequest(WWO_API_PATH + "?q={location}", Method.GET);
            rReq.AddUrlSegment("location", localValue);
            //rReq.AddParameter
            if (startDateValue != null)
            
            if (endDateValue != null)
            {
                rReq.Resource += '?';
            }
        }
    }
}

