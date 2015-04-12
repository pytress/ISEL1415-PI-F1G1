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
        const string API_PATH = "free/v2/past-weather.ashx";
        const string API_KEY = "e36e230efd71f15bbc15a97c39c38";
        const string FORMAT = "json";
        static readonly string[] validKeys = { "-local", "-startdate", "-enddate" };

        private string localValue;
        private string startDateValue;
        private string endDateValue;

        private RestClient rClient;
        private RestRequest rReq;
        //private RestResponse rResp;
        private string rRespContent;

        public WWOClient(IDictionary<string, string> keyValueArgs)
        {
            localValue = keyValueArgs[validKeys[0]];
            if (keyValueArgs.ContainsKey(validKeys[1]))
                startDateValue = keyValueArgs[validKeys[1]];
            if (keyValueArgs.ContainsKey(validKeys[2]))
                endDateValue = keyValueArgs[validKeys[2]];

            rClient = new RestClient(SCHEMA + HOST);
        }

        public void RequestData()
        {
            rReq = new RestRequest(API_PATH);
            rReq.RootElement = "data";
            rReq.RequestFormat = DataFormat.Json;

            //Build query string
            rReq.AddQueryParameter("key", API_KEY);
            rReq.AddQueryParameter("q", localValue);
            rReq.AddQueryParameter("format", FORMAT);
            rReq.AddQueryParameter("date", "2015-04-10");
            rReq.AddQueryParameter("enddate", "2015-04-11");
            //rReq.AddQueryParameter("tp", "24");

            //rReq.AddParameter
            //if (startDateValue != null)
            //{
            //}
            //if (endDateValue != null)
            //{
            //    rReq.Resource += '?';
            //}

            Console.WriteLine(rClient.BuildUri(rReq));
            //RestResponse rResp = (RestResponse)rClient.Execute(rReq);

            var rResp = rClient.Execute<NewWWOData>(rReq);

            rRespContent = rResp.Content;
            Console.WriteLine(rRespContent);

            NewWWOData wwoData = new NewWWOData();
            wwoData = rResp.Data;
            Console.WriteLine(wwoData.data);
            Console.WriteLine(wwoData.data.request);
        }
    }
}

