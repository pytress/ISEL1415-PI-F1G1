using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class Histogram
    {
        private class TemperatureOccurences
        {
            internal TemperatureOccurences()
            {
                minNOccurences = maxNOccurences = 0;
            }

            internal TemperatureOccurences(int tMin, int tMax)
            {
                minNOccurences = tMin; maxNOccurences = tMin;
            }

            private int minNOccurences;
            protected int MinNOccurences { get { return minNOccurences; } }
            private int maxNOccurences;
            protected int MaxNOccurences { get { return maxNOccurences;} }

            internal void IncMinOcc() { ++minNOccurences; }
            internal void IncMaxOcc() { ++maxNOccurences; }
        }

        private class HourlyAvgTemperature
        {
            internal HourlyAvgTemperature(int hour, int avgT)
            {
                this.hour = hour; avgTemperature = avgT;
            }

            private int hour;
            protected int Hour { get { return hour; } }
            private int avgTemperature;
            protected int AvgTemperature { get { return avgTemperature; } }
        }

        const char TEMP_MIN = '*';
        const char TEMP_MAX = '#';

        private string local;
        public string Local { get { return local; } }

        private DateTime dateFrom;
        private DateTime dateTo;
        public string DateInterval
        {
            get { return "From " + dateFrom.ToString("yyyy-MM-dd") + " To " + dateTo.ToString("yyyy-MM-dd"); }
        }

        private SortedList<int, TemperatureOccurences> infoBars;
        private Dictionary<DateTime, List<HourlyAvgTemperature>> avgTempsHourlyByDay;

        //private Data data {get;set;}

        public Histogram(WeatherData wData)
        {
            foreach (Data data in wData.GetDataItems())
                GetNecessaryInfo(data);
        }

        internal void Present()
        {
            throw new NotImplementedException();
        }

        private void GetNecessaryInfo(Data data)
        {
            if (local == null)
                local = data.ReferenceLocal;
            else
                if (local != data.ReferenceLocal)
                    throw new ApplicationException("### ERROR: Weather data inconsistent!");

            if (dateFrom == null) return;
                dateFrom = DateTime.Parse(data.Date);
            dateTo = DateTime.Parse(data.Date);

            AddTemperatureInfo(data.weather);
        }

        private void AddTemperatureInfo(List<Weather> wList)
        {
            foreach (Weather w in wList)
            {
                //FILL INFOBARS
                int idxMin = infoBars.IndexOfKey(int.Parse(w.mintempC));
                int idxMax = infoBars.IndexOfKey(int.Parse(w.maxtempC));

                if (idxMin < 0) //in case there isn't already this minimum temperature registered
                    infoBars.Add(int.Parse(w.mintempC), new TemperatureOccurences(1, 0));
                else            //in case there is already this minimum temperature registered
                    infoBars[idxMin].IncMinOcc();

                if (idxMax < 0) //in case there isn't already this minimum temperature registered
                    infoBars.Add(int.Parse(w.maxtempC), new TemperatureOccurences(0, 1));
                else            //in case there is already this minimum temperature registered
                    infoBars[idxMax].IncMinOcc();

                //FILL HOURLYAVGTEMPSHOURLYBYDAY
                List<HourlyAvgTemperature> avgTemps = new List<HourlyAvgTemperature>();

                foreach (Hourly h in w.hourly)
                    avgTemps.Add(new HourlyAvgTemperature(int.Parse(h.time), int.Parse(h.tempC)));

                avgTempsHourlyByDay.Add(DateTime.Parse(w.date), avgTemps);
            }
        }
    }
}
