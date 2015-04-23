using System;
using System.Collections.Generic;

namespace Ficha1
{
    public class Request
    {
        public string query { get; set; }
        public string type { get; set; }
    }

    public class Astronomy
    {
        public string moonrise { get; set; }
        public string moonset { get; set; }
        public string sunrise { get; set; }
        public string sunset { get; set; }
    }

    public class WeatherDesc
    {
        public string value { get; set; }
    }

    public class WeatherIconUrl
    {
        public string value { get; set; }
    }

    public class Hourly
    {
        public string cloudcover { get; set; }
        public string DewPointC { get; set; }
        public string DewPointF { get; set; }
        public string FeelsLikeC { get; set; }
        public string FeelsLikeF { get; set; }
        public string HeatIndexC { get; set; }
        public string HeatIndexF { get; set; }
        public string humidity { get; set; }
        public string precipMM { get; set; }
        public string pressure { get; set; }
        public string tempC { get; set; }
        public string tempF { get; set; }
        public string time { get; set; }
        public string visibility { get; set; }
        public string weatherCode { get; set; }
        public List<WeatherDesc> weatherDesc { get; set; }
        public List<WeatherIconUrl> weatherIconUrl { get; set; }
        public string WindChillC { get; set; }
        public string WindChillF { get; set; }
        public string winddir16Point { get; set; }
        public string winddirDegree { get; set; }
        public string WindGustKmph { get; set; }
        public string WindGustMiles { get; set; }
        public string windspeedKmph { get; set; }
        public string windspeedMiles { get; set; }
    }

    public class Weather
    {
        public List<Astronomy> astronomy { get; set; }
        public string date { get; set; }
        public List<Hourly> hourly { get; set; }
        public string maxtempC { get; set; }
        public string maxtempF { get; set; }
        public string mintempC { get; set; }
        public string mintempF { get; set; }
        public string uvIndex { get; set; }
    }

    public class Data
    {
        public List<string> error { get; set; }
        public List<Request> request { get; set; }
        public List<Weather> weather { get; set; }

        public string ReferenceLocal { get { return request[0].query; } }
        public string FirstDate { get { return weather[0].date; } }
        public string LastDate { get { return weather[weather.Count - 1].date; } }

        internal void ShowContent() //DEBUG: to show Data content
        {
            string firstDay = null, lastDay = null;
            
            Console.WriteLine("------------------------------------");
            request.ForEach(rq => Console.WriteLine("Local: {0}", rq.query));
            weather.ForEach(wt => {
                if (firstDay == null)
                    firstDay = wt.date;
                lastDay = wt.date;
                Console.WriteLine(" Date: {0}", wt.date);
                Console.WriteLine("  Max: {0}C", wt.maxtempC);
                Console.WriteLine("  Min: {0}C", wt.mintempC);
                wt.hourly.ForEach(h => Console.WriteLine("     At {0}hours : {1}C", h.time, h.tempC));
            });
            Console.WriteLine("------------------------------------");
            Console.WriteLine("First day was {0} and last day was {1}; {2} days", firstDay, lastDay, weather.Count);
        }
    }

    public class WeatherData
    {
        private List<Data> weatherData;

        public WeatherData()
        {
            weatherData = new List<Data>();
        }

        public bool IsEmpty
        {
            get { return weatherData.Count == 0 ? true : false; }
        }

        public void Append(Data wwoData)
        {
            weatherData.Add(wwoData);
        }

        public IEnumerable<Data> GetDataItems()
        {
            foreach (Data data in weatherData)
                yield return data;
        }
    }
}
