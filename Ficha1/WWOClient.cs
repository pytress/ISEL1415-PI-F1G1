using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
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
        //static readonly HashSet<string> validKeys2 = new HashSet<string> { "-local", "-startdate", "-enddate" };

        //private string localValue;
        //private string startDateValue;
        //private string endDateValue;
        private IDictionary<string, string> usefullArgPairs;

        private RestClient rClient;
        private RestRequest rReq;
        //private RestResponse rResp;
        //private string rRespContent;

        private string reqResultStatus;
        public string ReqResultStatus { get { return reqResultStatus; } }

        /*
         * Creates instance by matching pair values to correspondent specific keys if they exist
         * It ignores any other other key value pair
         */
        public WWOClient(IDictionary<string, string> keyValueArgs)
        {
            usefullArgPairs = keyValueArgs.Where(kVPair => validKeys.Contains(kVPair.Key))
                                        .ToDictionary(kVPair => kVPair.Key, kVPair => kVPair.Value);

            /*
            localValue = keyValueArgs[validKeys[0]];
            if (keyValueArgs.ContainsKey(validKeys[1]))
                startDateValue = keyValueArgs[validKeys[1]];
            if (keyValueArgs.ContainsKey(validKeys[2]))
                endDateValue = keyValueArgs[validKeys[2]];
            */
            rClient = new RestClient(SCHEMA + HOST);
        }

        public void RequestData()
        {
            rReq = new RestRequest(API_PATH);
            rReq.RequestFormat = DataFormat.Json; //TODO: necessary?
            rReq.RootElement = "data";

            //Build query string with mandatory parameters
            rReq.AddQueryParameter("key", API_KEY);        //registered key to access API
            //rReq.AddQueryParameter("q", localValue);       //local mandatory argument
            rReq.AddQueryParameter("q", usefullArgPairs[validKeys[0]]); //local mandatory argument            
            rReq.AddQueryParameter("format", RESP_FORMAT); //desired format for data requested
            //rReq.AddQueryParameter("tp", "24"); //DEBUG: for test purposes

            AddOptionalParameters();

            Console.WriteLine(rClient.BuildUri(rReq)); //DEBUG: print request URI
            //RestResponse rResp = (RestResponse)rClient.Execute(rReq);

            var rResp = ExecuteRequest();
            //Data wwoData = ExecuteRequest(); //ALTERNATIVA: passar parte do codigo abaixo para este metodo novo auxiliar

            //rRespContent = rResp.Content; //DEBUG: get HTTP response body
            //Console.WriteLine(rRespContent); //DEBUG: print HTTP response body

            Data wwoData;
            if (rResp != null)
            {
                wwoData = rResp.Data;
                wwoData.ShowContent(); //DEBUG: to see what is the data received
            }
            //parece que todos os testes feito por aqui resultam em content-encoding gzip (not transfer-enconding chunked)
            //ao passo que nos testes do proprio site do WWO costuma ser transfer-enconding chunked
            //parece ainda que o maximo de dias que devolve num unico pedido sao 35 dias
        }

        private IRestResponse<Data> ExecuteRequest()
        {
            rReq.OnBeforeDeserialization = rsp => Console.WriteLine(" ### BANG! Before deserialization. ### "); //DEBUG: test method execution before deserialization
            var resp = rClient.Execute<Data>(rReq);

            Console.WriteLine("Response :: HTTP Status Code = {0}", resp.StatusCode); //DEBUG: show HTTP status code returned by server

            if (resp.ResponseStatus == ResponseStatus.Error)
                if (resp.StatusCode == HttpStatusCode.BadRequest) //i've only checked this case with deserialization error/problem
                {
                    Console.WriteLine("### MSG: Client error - Bad Request"); //DEBUG
                    reqResultStatus = "400 Bad Request";
                    return null;
                }
                else
                {
                    throw new ApplicationException("### ERROR: RestSharp or network problem! (Description msg: " + resp.ErrorMessage + ")");
                }

            if (resp.ResponseStatus == ResponseStatus.Aborted || resp.ResponseStatus == ResponseStatus.TimedOut) //TODO: exception necessary? should we just warn user?
                throw new ApplicationException("### ERROR: RestSharp or network problem! (Description msg: " + resp.ErrorMessage + ")");

            if (resp.ResponseStatus == ResponseStatus.Completed)
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("### MSG: Request OK, should have return data."); //DEBUG
                    reqResultStatus = "200 OK";
                }
                else
                {
                    Console.WriteLine("### MSG: {0}", resp.StatusDescription); //DEBUG
                    reqResultStatus = resp.StatusCode.ToString();
                }
            //resp.ResponseStatus can still got None
            
            //Console.WriteLine("Response :: Content = {0}", resp.Content);
            Console.WriteLine("Response :: Exception = {0}", resp.ErrorException);
            Console.WriteLine("Response :: Message = {0}", resp.ErrorMessage);

            return resp;
        }

        private void AddOptionalParameters() //TODO: the relationship between command line parameter and WWO API parameter strings should be better
        {
            /*
            if (startDateValue != null)                                              //start date is defined
            {
                rReq.AddQueryParameter("date", startDateValue);
                if (endDateValue != null)                                            //and end date is also defined
                    rReq.AddQueryParameter("enddate", endDateValue);
            }
            else                                                                     //start date is not defined
            {
                rReq.AddQueryParameter("date", DateTime.Now.ToString("yyyy-MM-dd")); //then start date is current day
                if (endDateValue != null)                                            //but end date is (the only defined)
                    rReq.AddQueryParameter("enddate", endDateValue);
            }
            */

            string startDate, endDate;

            if (usefullArgPairs.TryGetValue(validKeys[1], out startDate))            //start date is defined
            {
                rReq.AddQueryParameter("date", startDate);
                if (usefullArgPairs.TryGetValue(validKeys[2], out endDate))          //and end date is also defined
                    rReq.AddQueryParameter("enddate", endDate);
            }
            else                                                                         //start date is not defined
            {
                if (usefullArgPairs.TryGetValue(validKeys[2], out endDate))              //but end date is (the only defined)
                    rReq.AddQueryParameter("date", endDate);                             //then end date will be used as the only and start date
                else                                                                     //no date whatsoever defined
                    rReq.AddQueryParameter("date", DateTime.Now.ToString("yyyy-MM-dd")); //then start date is current day
            }
        }
    }
}

