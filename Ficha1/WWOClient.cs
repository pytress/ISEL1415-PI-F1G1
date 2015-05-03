using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string ALT_API_KEY = "085d2036a11f120f140e651c821b5";
        private const string RESP_FORMAT = "json";
        private const int DEFAULT_TIME_INTERVAL = 3;
        private const int MAX_N_DAYS_PER_REQ = 10;
        private const int QRY_PER_SEC_ALLOWED = 5;
        private const int MS_PAUSE = 1000;
        private const int TIMEOUT = 5000;
        public const string DATE_FORMAT = "yyyy-MM-dd";
        #endregion

        private static readonly string[] validKeys = { "-local", "-startdate", "-enddate" };

        private IDictionary<string, string> usefullArgPairs;

        private RestClient rClient;
        private WeatherData returnedData;
        public WeatherData ReturnedData { get { return returnedData; } }

        private volatile string lastReqResultStatus;
        public string LastReqResultStatus { get { return lastReqResultStatus; } }

        /*
         * Creates instance by matching pair values to correspondent specific keys if they exist
         * It ignores any other other key value pair
         */
        public WWOClient(IDictionary<string, string> keyValueArgs)
        {
            usefullArgPairs = keyValueArgs.Where(kVPair => validKeys.Contains(kVPair.Key))
                                          .ToDictionary(kVPair => kVPair.Key, kVPair => kVPair.Value);

            returnedData = new WeatherData();
            rClient = new RestClient(SCHEMA + HOST);
        }

        public HistAndGraphData GetData()
        {
            //Get start date and number of days
            DateTime start = GetStartDate();
            int nDays = GetNDaysRequested();

            HistAndGraphData tmpData = ProcessRequests(start, nDays);

            if (tmpData != null) //if the request returned some data
                tmpData.CalculateAvg(); //calculate media

            return tmpData;
        }

        private HistAndGraphData ProcessRequests(DateTime startDate, int nDays)
        {
            if (nDays <= MAX_N_DAYS_PER_REQ)
            {
                //Make HTTP request
                List<Weather> weather = RequestData(startDate, nDays);

                if (weather == null) return null;

                //Process data and return
                return ProcessReceivedData(weather);
            }
            else
            {
                //Split number of days in half
                int days = nDays / 2;                                      
                HistAndGraphData[] hData = new HistAndGraphData[2];

                Parallel.For(0, 2, i => {
                    //Start date depends on iteration; first half interval is always the size of the integer division
                    //Number of days can be diferent for the second half interval (if number of days is odd)
                    DateTime adjustedStart = startDate.AddDays(days * i); 
                    int adjustedDays = days + (nDays % 2) * i;            

                    hData[i] = ProcessRequests(adjustedStart, adjustedDays);
                });

                if (hData[0] == null && hData[1] == null) return null;
                if (hData[0] == null) return hData[1];
                if (hData[1] == null) return hData[0];
                
                return HistAndGraphData.Merge(hData);
            }
        }
        
        public List<Weather> RequestData(DateTime startDate, int nDays)
        {
            var request = new RestRequest(API_PATH);
            request.RequestFormat = DataFormat.Json; //just to make sure
            request.RootElement = "data";

            //Build query string
            request.AddQueryParameter("key", ALT_API_KEY);                 //registered key to access API
            request.AddQueryParameter("q", usefullArgPairs[validKeys[0]]); //local mandatory argument            
            request.AddQueryParameter("format", RESP_FORMAT);              //desired format for data requested
            request.AddQueryParameter("date", startDate.ToString(DATE_FORMAT));
            request.AddQueryParameter("enddate", startDate.AddDays(nDays-1).ToString(DATE_FORMAT));

            var rResp = ExecuteRequest(request);
            if (rResp == null) return null; //No data to desserialize; happens when WWO API returns 400 Bad Request

            while (lastReqResultStatus == "429") //429 Too Many Requests (WWO API Sucks!); there is no HttpStatusCode for 429
            {
                Thread.Sleep(MS_PAUSE);
                rResp = ExecuteRequest(request);
            }
            if (rResp == null) return null;

            Data wwoData = rResp.Data;
            if (ErrorInBody(wwoData))
                return null;

            return wwoData.weather;
        }

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

            return timeSpan.Days + 1;
        }

        private IRestResponse<Data> ExecuteRequest(RestRequest rReq)
        {
            var resp = rClient.Execute<Data>(rReq);

            if (resp.ResponseStatus == ResponseStatus.Error)
                if (resp.StatusCode == HttpStatusCode.BadRequest) //Only happens with deserialization error/problem
                {
                    lastReqResultStatus = "400 Bad Request";
                    return null;
                }
                else
                    throw new ApplicationException("### ERROR: RestSharp or network problem! (Description msg: " + resp.ErrorMessage + ")");

            if (resp.ResponseStatus == ResponseStatus.Aborted || resp.ResponseStatus == ResponseStatus.TimedOut)
                throw new ApplicationException("### ERROR: RestSharp or network problem! (Description msg: " + resp.ErrorMessage + ")");

            if (resp.ResponseStatus == ResponseStatus.Completed)
                if (resp.StatusCode == HttpStatusCode.OK)
                    lastReqResultStatus = "200 OK";
                else
                    lastReqResultStatus = resp.StatusCode.ToString();

            return resp;
        }

        private bool ErrorInBody(Data data)
        {
            if (data == null) return true;

            if (data!= null && data.error == null) return false;

            //Console.WriteLine("### ERROR: Request & Reply OK, but no valid data received!"); //DEBUG
            //Console.WriteLine("### ERROR: Message from server - {0}", data.error[0].msg);    //DEBUG
            
            lastReqResultStatus = "404 Not Found"; //assume this problema as a 404

            return true;
        }

        private HistAndGraphData ProcessReceivedData(List<Weather> wData)
        {
            //Create new HistAndGraphData object
            string local = usefullArgPairs[validKeys[0]]; //mandatory argument (local)
            string startDate = wData[0].date;
            string endDate = wData[wData.Count - 1].date;
            int nHours = wData[0].hourly.Count;
            HistAndGraphData hgData = new HistAndGraphData(startDate, endDate, nHours, local);

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
