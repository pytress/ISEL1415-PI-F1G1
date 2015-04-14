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
        const string RESP_FORMAT = "json";
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
            //rReq.RequestFormat = DataFormat.Json; //TODO: unnecessary?

            //Build query string
            rReq.AddQueryParameter("key", API_KEY);
            rReq.AddQueryParameter("q", localValue);
            rReq.AddQueryParameter("format", RESP_FORMAT);
            rReq.AddQueryParameter("date", "2015-01-06");    //DEBUG: for test purposes
            rReq.AddQueryParameter("enddate", "2015-03-22"); //DEBUG: for test purposes
            //Este ultimo intervalo, aparentemente resultou no WWOData vazio
            //rReq.AddQueryParameter("tp", "24"); //DEBUG: for test purposes

            /*
            rReq.AddParameter
            if (startDateValue != null)
            {
            }
            if (endDateValue != null)
            {
                rReq.Resource += '?';
            }
            */

            Console.WriteLine(rClient.BuildUri(rReq)); //DEBUG: print request URI
            //RestResponse rResp = (RestResponse)rClient.Execute(rReq);

            var rResp = rClient.Execute<Data>(rReq);

            rRespContent = rResp.Content;
            //Console.WriteLine(rRespContent); //DEBUG: print HTTP response body

            Data wwoData = rResp.Data;
            wwoData.ShowContent(); //DEBUG: to see what is the data received
            //parece que todos os testes feito por aqui resultam em content-encoding gzip (not transfer-enconding chunked)
            //ao passo que nos testes do proprio site do WWO costuma ser transfer-enconding chunked
            //parece ainda que o maximo de dias que devolve num unico pedido sao 35 dias
        }
    }
}

