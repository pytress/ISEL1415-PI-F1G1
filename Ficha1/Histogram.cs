using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class Histogram
    {
        const char TEMP_MIN = '*';
        const char TEMP_MAX = '#';
        const char AVG_TEMP = '>';

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
                return "interval from " + dateFrom.ToString("yyyy-MM-dd") + " To " + dateTo.ToString("yyyy-MM-dd");
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
                Console.Write("{0} | ", kVElem.Key);
                string str1 = " " + Convert.ToString(kVElem.Value.MinNOccurences);
                Console.WriteLine(str1.PadLeft(kVElem.Value.MinNOccurences, TEMP_MIN));

                Console.Write("{0} | ", kVElem.Key);
                string str2 = " " + Convert.ToString(kVElem.Value.MaxNOccurences);
                Console.WriteLine(str2.PadLeft(kVElem.Value.MaxNOccurences, TEMP_MAX));
                    //nMin & nMax:
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
                
                Console.WriteLine(str.PadLeft(avgTemp, AVG_TEMP));
            }

            Console.WriteLine("");
        }
        /*
        public static void Main()
        {
            string str = "forty-two";
            char pad = '.';

            Console.WriteLine(str.PadLeft(15, pad));
            Console.WriteLine(str.PadLeft(2, pad));
        }
        // The example displays the following output: 
        //       ......forty-two 
        //       forty-two
        */
    }
}
