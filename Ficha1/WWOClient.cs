using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using RestSharp;
using System.Threading.Tasks;

namespace Ficha1
{
    class WWOClient
    {
        #region Constants
        private const string SCHEMA = "http://";
        private const string HOST = "api.worldweatheronline.com";
        private const string API_PATH = "free/v2/past-weather.ashx";
        private const string API_KEY = "e36e230efd71f15bbc15a97c39c38";
        private const string RESP_FORMAT = "json";
        private const int DEFAULT_TIME_INTERVAL = 3;
        private const int MAX_N_DAYS_PER_REQ = 5;
        private const int QRY_PER_SEC_ALLOWED = 5;
        private const int MS_PAUSE = 1000;
        private const int TIMEOUT = 5000;
        public const string DATE_FORMAT = "yyyy-MM-dd";
        #endregion

        private static readonly string[] validKeys = { "-local", "-startdate", "-enddate" };
        //private static readonly HashSet<string> validKeys2 = new HashSet<string> { "-local", "-startdate", "-enddate" };

        //private string localValue;
        //private string startDateValue;
        //private string endDateValue;
        private IDictionary<string, string> usefullArgPairs;

        private RestClient rClient;
        //private RestRequest rReq;
        private WeatherData returnedData;
        public WeatherData ReturnedData { get { return returnedData; } }
        //private RestResponse rResp;
        //private string rRespContent;

        private string lastReqResultStatus;
        public string LastReqResultStatus { get { return lastReqResultStatus; } }

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

        public HistAndGraphData GetData()
        {
            //Get start date and number of days
            DateTime start = GetStartDate();
            int nDays = GetNDaysRequested();

            HistAndGraphData tmpData = ProcessRequests(start, nDays);

            //Calculate media
            tmpData.CalculateAvg();

            return tmpData;
        }

        private HistAndGraphData ProcessRequests(DateTime startDate, int nDays)
        {
            if (nDays <= MAX_N_DAYS_PER_REQ)
            {
                //make http request
                List<Weather> weather = RequestData(startDate, nDays);

                //Process data and return
                return ProcessReceivedData(weather);
            }
            else
            {
                //split number of days in half
                int days = nDays / 2;                                      
                HistAndGraphData[] hData = new HistAndGraphData[2];

                //Parallel.For(0, 2, i =>
                for(int i = 0; i < 2; ++i)
                {
                    //start depends on iteration; first half interval is always the size of the integer division
                    //the number of days can be diferent for the second half interval (if number of days is odd)
                    DateTime adjustedStart = startDate.AddDays(days * i); 
                    int adjustedDays = days + (nDays % 2) * i;            

                    hData[i] = ProcessRequests(adjustedStart, adjustedDays);
                }//);

                return HistAndGraphData.Merge(hData);
            }
            
        }
        
        //public void RequestAsyncData()
        //{
        //    //number of HTTP request necessary to obtain the data requested
        //    int nReq = (GetNDaysRequested() + MAX_N_DAYS_PER_REQ - 1) / MAX_N_DAYS_PER_REQ;

        //    //Create list of requests
        //    WWOAsyncRequests requestList = new WWOAsyncRequests();

        //    for (int i = 0; i < nReq; ++i)
        //    {
        //        var request = new RestRequest(API_PATH);
        //        request.RequestFormat = DataFormat.Json; //TODO: necessary?
        //        request.RootElement = "data";

        //        //Build query string with mandatory parameters
        //        request.AddQueryParameter("key", API_KEY);        //registered key to access API
        //        request.AddQueryParameter("q", usefullArgPairs[validKeys[0]]); //local mandatory argument            
        //        request.AddQueryParameter("format", RESP_FORMAT); //desired format for data requested
        //        AddOptionalParameters(request);

        //        //to avoid status error 429 Too Many Requests
        //        if (i % QRY_PER_SEC_ALLOWED == 0)
        //            Thread.Sleep(MS_PAUSE);

        //        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>  THE MAGIC IS HERE :D <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        //        //Make asynchronous request
        //        RestRequestAsyncHandle asyncHandle = null;
        //        asyncHandle = rClient.ExecuteAsync<Data>(request, response => {
        //            requestList.SaveRequest(asyncHandle, response);
        //        });
        //        requestList.SaveRequest(asyncHandle, null);
        //    }
            
        //    //Wait for requests to finish
        //    Console.WriteLine("Waiting for requests");
        //    if (!requestList.WaitForFinish(TIMEOUT))
        //    {
        //        Console.WriteLine("WARNING: Requests are taking too long to complete.");
        //        requestList.CancelRequests();
        //        //TODO what else?
        //        //TODO test with small amount of time to force timeout, and see what happens
        //    }

        //    //Check if all requests completed successfully
        //    //TODO if not ok?
        //    if (!requestList.CheckStatusCodes())
        //        Console.WriteLine("WARNING: Not every request completed successfully.");

        //    //Consolidate all responses
        //    foreach (RestResponse<Data> r in requestList.RequestDict.Values)
        //    {
        //        if (!ErrorInBody(r.Data))
        //            returnedData.Append(r.Data);
        //    }

        //}

        public List<Weather> RequestData(DateTime startDate, int nDays)
        {

            var request = new RestRequest(API_PATH);
            request.RequestFormat = DataFormat.Json; //TODO: necessary?
            request.RootElement = "data";

            //Build query string
            request.AddQueryParameter("key", API_KEY);        //registered key to access API
            request.AddQueryParameter("q", usefullArgPairs[validKeys[0]]); //local mandatory argument            
            request.AddQueryParameter("format", RESP_FORMAT); //desired format for data requested
            request.AddQueryParameter("date", startDate.ToString(DATE_FORMAT));
            request.AddQueryParameter("enddate", startDate.AddDays(nDays).ToString(DATE_FORMAT));

            var rResp = ExecuteRequest(request);
            
            if (rResp != null)
            {
                Data wwoData = rResp.Data;
                if (!ErrorInBody(wwoData))
                {
                    return wwoData.weather;
                }
            }

            return null;
        }

        //TODO: this method was replaced by RequestAsyncData(); remove this one.
        //NOTA: so disponivel dados de 1-Jul-2008 em diante; agora parece que so de ha 2 meses a esta parte
        //NOTA: exemplo do resultado: { "data": { "error": [ {"msg": "There is no weather data available for the date provided. Past data is available from 1 July, 2008 onwards only." } ] }}
        //public void RequestData()
        //{
        //    //number of HTTP request necessary to obtain the data requested
        //    int nReq = (GetNDaysRequested() + MAX_N_DAYS_PER_REQ - 1) / MAX_N_DAYS_PER_REQ;
            
        //    //TODO DEBUG: show number of requests necessary to obtain all requested data
        //    Console.WriteLine("Number of requests necessary: {0}", nReq); 

        //    for (int i = 0; i < nReq; ++i) //many requests as necessary to obtain all requested data
        //    {
        //        var rReq = new RestRequest(API_PATH);
        //        rReq.RequestFormat = DataFormat.Json; //TODO: necessary?
        //        rReq.RootElement = "data";

        //        //Build query string with mandatory parameters
        //        rReq.AddQueryParameter("key", API_KEY);        //registered key to access API
        //        //rReq.AddQueryParameter("q", localValue);       //local mandatory argument
        //        rReq.AddQueryParameter("q", usefullArgPairs[validKeys[0]]); //local mandatory argument            
        //        rReq.AddQueryParameter("format", RESP_FORMAT); //desired format for data requested
        //        //rReq.AddQueryParameter("tp", "24"); //DEBUG: for test purposes

        //        //AddOptionalParameters();
        //        AddOptionalParameters(rReq);

        //        //TODO DEBUG: print request URI
        //        Console.WriteLine(rClient.BuildUri(rReq));


        //        //TODO: VERIFICAR CASOS DE 200 OK COM ERRO NO BODY (caso de datas anteriores a 2008)
        //        //ALTERNATIVA: passar parte do codigo abaixo para este metodo novo auxiliar
        //        var rResp = ExecuteRequest(rReq);

        //        Data wwoData;
        //        if (rResp != null)
        //        {
        //            wwoData = rResp.Data;
        //            if (!ErrorInBody(wwoData))
        //            {
        //                //wwoData.ShowContent(); //DEBUG: to see what is the data received
        //                returnedData.Append(wwoData);
        //            }
        //        }

        //        if (i % QRY_PER_SEC_ALLOWED == 0) //to avoid status error 429 Too Many Requests
        //            Thread.Sleep(MS_PAUSE);

        //        //TODO: parece que todos os testes feito por aqui resultam em content-encoding gzip (not transfer-enconding chunked)
        //        //ao passo que nos testes do proprio site do WWO costuma ser transfer-enconding chunked
        //        //parece ainda que o maximo de dias que devolve num unico pedido sao 35 dias
        //    }
        //}

        private DateTime GetStartDate()
        {
            string start;
            DateTime dtStart;
            if (usefullArgPairs.TryGetValue(validKeys[1], out start))
                dtStart = DateTime.Parse(start);
            else
                dtStart = DateTime.Now;

            return dtStart;
        }

        private int GetNDaysRequested()
        {
            string start, end;

            if (!usefullArgPairs.TryGetValue(validKeys[1], out start) || !usefullArgPairs.TryGetValue(validKeys[2], out end))
                return 1;

            TimeSpan timeSpan = DateTime.Parse(end) - DateTime.Parse(start);
            int nDays = timeSpan.Days == 0 ? 1 : timeSpan.Days;

            //DEBUG: show number of days requested
            //Console.WriteLine("Calculated number of days (in interval): {0}", nDays); 
            return nDays;
        }

        private IRestResponse<Data> ExecuteRequest(RestRequest rReq)
        {
            var resp = rClient.Execute<Data>(rReq);

            //DEBUG: show HTTP status code returned by server
            //Console.WriteLine("Response :: HTTP Status Code = {0}", resp.StatusCode);

            if (resp.ResponseStatus == ResponseStatus.Error)
                if (resp.StatusCode == HttpStatusCode.BadRequest) //i've only witnessed this case with deserialization error/problem
                {
                    //Console.WriteLine("### MSG: Client error - Bad Request"); //DEBUG
                    lastReqResultStatus = "400 Bad Request";
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
                    //Console.WriteLine("### MSG: Request OK, should have return data."); //DEBUG
                    lastReqResultStatus = "200 OK";
                }
                else
                {
                    //Console.WriteLine("### MSG: {0}", resp.StatusDescription); //DEBUG
                    lastReqResultStatus = resp.StatusCode.ToString();
                }

            return resp;
        }

        private bool ErrorInBody(Data data)
        {
            {
                if (data!= null && data.error == null)
                    return false;

                Console.WriteLine("### MSG: Request OK, but no valid data => {0}", 
                                    data==null?"":data.error.First()); //DEBUG
                
                lastReqResultStatus = "404 Not Found"; //TODO ??(HF)
                return true;
            }
        }

        /**
         //TODO: the relationship between command line parameter and WWO API parameter strings should be better
         */
        //private void AddOptionalParameters() 
        //private void AddOptionalParameters(RestRequest rReq) 
        //{
        //    /*
        //    if (startDateValue != null)                                              //start date is defined
        //    {
        //        rReq.AddQueryParameter("date", startDateValue);
        //        if (endDateValue != null)                                            //and end date is also defined
        //            rReq.AddQueryParameter("enddate", endDateValue);
        //    }
        //    else                                                                     //start date is not defined
        //    {
        //        rReq.AddQueryParameter("date", DateTime.Now.ToString("yyyy-MM-dd")); //then start date is current day
        //        if (endDateValue != null)                                            //but end date is (the only defined)
        //            rReq.AddQueryParameter("enddate", endDateValue);
        //    }
        //    */

        //    string start, end;

        //    if (usefullArgPairs.TryGetValue(validKeys[1], out start))            //start date is defined
        //    {
        //        rReq.AddQueryParameter("date", start);
        //        if (usefullArgPairs.TryGetValue(validKeys[2], out end))          //and end date is also defined
        //        {
        //            DateTime partialEndDate = DateTime.Parse(start).AddDays(MAX_N_DAYS_PER_REQ - 1); //set new end date to start plus the maximum number of days per request
        //            DateTime partialStartDate = partialEndDate.AddDays(1);                           //set new start date (day before new end date)

        //            usefullArgPairs[validKeys[1]] = partialStartDate.ToString(DATE_FORMAT);         //save new start date to arguments
        //            Console.WriteLine("New start date: {0}", usefullArgPairs[validKeys[1]]);       //DEBUG: show new start date
        //            if (partialEndDate > DateTime.Parse(end))                                        //check end of requeste date interval
        //                rReq.AddQueryParameter("enddate", end);
        //            else
        //            {
        //                rReq.AddQueryParameter("enddate", partialEndDate.ToString(DATE_FORMAT));
        //                Console.WriteLine("New end date: {0}", partialEndDate.ToString(DATE_FORMAT)); //DEBUG: show new start date
        //            }
        //        }
        //    }
        //    else                                                                         //start date is not defined
        //    {
        //        if (usefullArgPairs.TryGetValue(validKeys[2], out end))                  //but end date is (the only defined)
        //            rReq.AddQueryParameter("date", end);                                 //then end date will be used as the only and start date
        //        else                                                                     //no date whatsoever defined
        //            rReq.AddQueryParameter("date", DateTime.Now.ToString(DATE_FORMAT)); //then start date is current day
        //    }
        //}

        private HistData FilterWDataForHist(List<Weather> wData) //TODO: necessário confirmar conteúdo válido antes de evocar
        {
            HistData hData = new HistData(wData[0].date, wData[wData.Count - 1].date);

            wData.ForEach(elem => hData.AddTemps(int.Parse(elem.mintempC), int.Parse(elem.maxtempC)));

            return hData;
        }

        private GraphDataChunk FilterWDataForGraph(List<Weather> wData) //TODO: necessário confirmar conteúdo válido antes de evocar
        {
            GraphDataChunk gData = new GraphDataChunk(wData[0].date, wData[wData.Count - 1].date);

            foreach (Weather wElem in wData)
            {
                if (gData.SetDate(wElem.date) == true)
                    foreach (Hourly hourly in wElem.hourly)
                        gData.AddHourlyTemps(int.Parse(hourly.time), int.Parse(hourly.tempC));
            }

            return gData;
        }

        //TODO: necessário confirmar conteúdo válido antes de evocar
        //TODO: como verificar que o resultado corresponde ao pedido (interval de datas)
        private HistAndGraphData ProcessReceivedData(List<Weather> wData)
        {
            //Create new HistAndGraphData object
            string startDate = wData[0].date;
            string endDate = wData[wData.Count - 1].date;
            int nHours = wData[0].hourly.Count;
            HistAndGraphData hgData = new HistAndGraphData(startDate, endDate, nHours);


            wData.ForEach(wElem => {
                //Add daily temperatures
                int min = int.Parse(wElem.mintempC);
                int max = int.Parse(wElem.maxtempC);
                hgData.AddDailyTemps(min, max);


                //Add hourly temperatures
                foreach (Hourly hourly in wElem.hourly)
                {
                    int time = int.Parse(hourly.time);
                    int temp = int.Parse(hourly.tempC);
                    hgData.AddHourlyTemps(time,temp);
                }
            });
            
            return hgData;
        }
    }
}
