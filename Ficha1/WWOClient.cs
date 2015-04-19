﻿using System;
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
        const int MAX_N_DAYS_PER_REQ = 34;
        static readonly string[] validKeys = { "-local", "-startdate", "-enddate" };
        //static readonly HashSet<string> validKeys2 = new HashSet<string> { "-local", "-startdate", "-enddate" };

        //private string localValue;
        //private string startDateValue;
        //private string endDateValue;
        private IDictionary<string, string> usefullArgPairs;

        private RestClient rClient;
        private RestRequest rReq;
        private WeatherData returnedData;
        public WeatherData ReturnedData { get { return returnedData; } }
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

            returnedData = new WeatherData();
            rClient = new RestClient(SCHEMA + HOST);
        }

        //NOTA: so disponivel dados de 1-Jul-2008 em diante; agora parece que so de ha 2 meses a esta parte
        //NOTA: exemplo do resultado: { "data": { "error": [ {"msg": "There is no weather data available for the date provided. Past data is available from 1 July, 2008 onwards only." } ] }}
        public void RequestData()
        {
            //int nDaysReq = GetNDaysRequested();
            int nReq = (GetNDaysRequested() + MAX_N_DAYS_PER_REQ - 1) / MAX_N_DAYS_PER_REQ; //number of HTTP request necessary to obtain da data requested
            Console.WriteLine("Number of requests necessary: {0}", nReq); //DEBUG: show number of requests necessary to obtain all requested data

            for (int i = 0; i < nReq; ++i) //many requests as necessary to obtain all requested data
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
                    //wwoData.ShowContent(); //DEBUG: to see what is the data received
                    returnedData.Append(wwoData);
                }
                //parece que todos os testes feito por aqui resultam em content-encoding gzip (not transfer-enconding chunked)
                //ao passo que nos testes do proprio site do WWO costuma ser transfer-enconding chunked
                //parece ainda que o maximo de dias que devolve num unico pedido sao 35 dias
            }
        }

        private int GetNDaysRequested()
        {
            string start, end;

            if (!usefullArgPairs.TryGetValue(validKeys[1], out start) || !usefullArgPairs.TryGetValue(validKeys[2], out end))
                return 1;

            TimeSpan timeSpan = DateTime.Parse(end) - DateTime.Parse(start);

            Console.WriteLine("Calculated number of days (in interval): {0}", timeSpan.Days); //DEBUG: show number of days requested
            return timeSpan.Days;
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

            string start, end;

            if (usefullArgPairs.TryGetValue(validKeys[1], out start))            //start date is defined
            {
                rReq.AddQueryParameter("date", start);
                if (usefullArgPairs.TryGetValue(validKeys[2], out end))          //and end date is also defined
                {
                    rReq.AddQueryParameter("enddate", end);
                    //DateTime newStartDate = DateTime.Parse(start).AddDays(MAX_N_DAYS_PER_REQ);
                    usefullArgPairs[validKeys[1]] = DateTime.Parse(start).AddDays(MAX_N_DAYS_PER_REQ + 1).ToString("yyyy-MM-dd");
                    Console.WriteLine("New start date: {0}", usefullArgPairs[validKeys[1]]); //DEBUG: show new start date
                }
            }
            else                                                                         //start date is not defined
            {
                if (usefullArgPairs.TryGetValue(validKeys[2], out end))                  //but end date is (the only defined)
                    rReq.AddQueryParameter("date", end);                                 //then end date will be used as the only and start date
                else                                                                     //no date whatsoever defined
                    rReq.AddQueryParameter("date", DateTime.Now.ToString("yyyy-MM-dd")); //then start date is current day
            }
        }
    }
}

