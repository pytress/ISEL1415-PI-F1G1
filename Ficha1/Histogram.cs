using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class Histogram
    {
        const char TEMP_MIN_CHR = '*';
        const char TEMP_MAX_CHR = '#';
        const char AVG_TEMP_CHR = '>';
        const string TOP = "@";

        private class TemperatureOccurences
        {
            internal TemperatureOccurences()
            {
                minNOccurences = maxNOccurences = 0;
            }
            internal TemperatureOccurences(int tMin, int tMax)
            {
                minNOccurences = tMin; maxNOccurences = tMax;
            }

            private int minNOccurences;
            internal int MinNOccurences { get { return minNOccurences; } }
            private int maxNOccurences;
            internal int MaxNOccurences { get { return maxNOccurences;} }

            internal void IncMinOcc() { ++minNOccurences; }
            internal void IncMaxOcc() { ++maxNOccurences; }
        }

        private class HourlyTemperature
        {
            internal HourlyTemperature(int hour, int avgT)
            {
                this.hour = hour; temperature = avgT;
            }

            private int hour;
            internal int Hour { get { return hour; } }
            private int temperature;
            internal int Temperature { get { return temperature; } }
        }

        private string local;
        public string Local { get { return local; } }

        private DateTime dateFrom;
        private DateTime dateTo;
        public string DateInterval()
        {
            if (dateFrom == dateTo)
                return dateTo.ToString("yyyy-MM-dd");
            else
                return "interval from " + dateFrom.ToString("yyyy-MM-dd") + " to " + dateTo.ToString("yyyy-MM-dd");
        }

        private SortedList<int, TemperatureOccurences> infoBars = new SortedList<int,TemperatureOccurences>();
        private Dictionary<DateTime, List<HourlyTemperature>> hourlyTempsByDay = new Dictionary<DateTime,List<HourlyTemperature>>();

        public Histogram(WeatherData wData)
        {
            foreach (Data data in wData.GetDataItems())
                GetNecessaryInfo(data);
        }

        internal void PresentResults()
        {
            PresentHistogram();
            PresentDayAvgs();
        }

        private void GetNecessaryInfo(Data data)
        {
            if (local == null) //for the first (or only) data group
                local = data.ReferenceLocal;
            else               //if there is more data groups local should be the same
                if (local != data.ReferenceLocal)
                    throw new ApplicationException("### ERROR: Weather data inconsistent!");

            if (dateFrom == default(DateTime))      //the requested start date comes only onithe first (or only) data group
                dateFrom = DateTime.Parse(data.FirstDate);
            dateTo = DateTime.Parse(data.LastDate); //the requested end date is the last one in the last data group

            AddTemperatureInfo(data.weather);
        }

        private void AddTemperatureInfo(List<Weather> wList)
        {
            foreach (Weather w in wList)
            {
                //Console.WriteLine("Min: {0} | Max: {1}", w.mintempC, w.maxtempC); //DEBUG
                //FILL INFOBARS
                if (infoBars.ContainsKey(int.Parse(w.mintempC))) //in case there is already this minimum temperature registered
                    infoBars[int.Parse(w.mintempC)].IncMinOcc();
                else                                             //in case there isn't already this minimum temperature registered
                    infoBars.Add(int.Parse(w.mintempC), new TemperatureOccurences(1, 0));

                if (infoBars.ContainsKey(int.Parse(w.maxtempC))) //in case there is already this maximum temperature registered
                    infoBars[int.Parse(w.maxtempC)].IncMaxOcc();
                else                                             //in case there isn't already this maximum temperature registered
                    infoBars.Add(int.Parse(w.maxtempC), new TemperatureOccurences(0, 1));

                //FILL HOURLYTEMPSBYDAY
                List<HourlyTemperature> tempsForADay = new List<HourlyTemperature>();

                foreach (Hourly h in w.hourly)
                    tempsForADay.Add(new HourlyTemperature(int.Parse(h.time), int.Parse(h.tempC)));

                hourlyTempsByDay.Add(DateTime.Parse(w.date), tempsForADay);
            }
        }

        private void PresentHistogram()
        {
            Console.WriteLine("Temperature Histogram for: {0}", DateInterval());
            Console.WriteLine();

            foreach (KeyValuePair<int, TemperatureOccurences> kVElem in infoBars)
            {
                string str1 = null, str2 = null;

                Console.Write("{0} | ", kVElem.Key);
                if (kVElem.Value.MinNOccurences != 0) //cosmetic only
                    str1 = " ";
                str1 += Convert.ToString(kVElem.Value.MinNOccurences);
                Console.WriteLine(str1.PadLeft(kVElem.Value.MinNOccurences * 3, TEMP_MIN_CHR));
                //else
                    //Console.WriteLine("");
                //Console.Write(TOP.PadLeft(kVElem.Value.MinNOccurences, TEMP_MIN_CHR));
                //Console.WriteLine(kVElem.Value.MinNOccurences);

                Console.Write("{0} | ", kVElem.Key);
                if (kVElem.Value.MaxNOccurences != 0)
                    str2 = " ";
                str2 += Convert.ToString(kVElem.Value.MaxNOccurences);
                Console.WriteLine(str2.PadLeft(kVElem.Value.MaxNOccurences * 3, TEMP_MAX_CHR)); //multiply by 3 to get more characters (longer line)
                //Console.Write(TOP.PadLeft(kVElem.Value.MaxNOccurences, TEMP_MAX_CHR));
                //Console.WriteLine(kVElem.Value.MaxNOccurences);

                //Console.WriteLine("");
            }

            Console.WriteLine("");
        }

        private void PresentDayAvgs() //TODO: apresentar algum tipo de referencia à quantidade de temperaturas horarias e/ou ao intervalo entre as mesmas?
        {
            Console.WriteLine("Daily Average Temperature for: {0}", DateInterval());
            Console.WriteLine();

            foreach (KeyValuePair<DateTime, List<HourlyTemperature>> kVElem in hourlyTempsByDay.Reverse())
            {
                Console.Write("{0} | ", kVElem.Key.ToString("yyyy-MM-dd"));

                int avgTemp = Convert.ToInt32(kVElem.Value.Average<HourlyTemperature>(hTemp => hTemp.Temperature));
                string str = " " + Convert.ToString(avgTemp);
                
                Console.WriteLine(str.PadLeft(avgTemp, AVG_TEMP_CHR));
            }

            Console.WriteLine("");
        }
    }
}
