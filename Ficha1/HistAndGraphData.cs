using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ficha1
{
    class HistAndGraphData
    {
        public class TemperatureOccurences
        {
            //number of a minimum temperature occurences
            private int minNOccurences; 
            internal int MinNOccurences { get { return minNOccurences; } }

            //number of a maximum temperature occurences
            private int maxNOccurences; 
            internal int MaxNOccurences { get { return maxNOccurences; } }

            internal TemperatureOccurences(int tMin, int tMax)
            {
                minNOccurences = tMin; maxNOccurences = tMax;
            }

            internal void IncMinOcc() { ++minNOccurences; }
            internal void IncMaxOcc() { ++maxNOccurences; }
        }

        private string local;
        public string Local { get { return local; } }

        private int dayCounter; //counts the number of data days the object contains
        public int NDays { get { return dayCounter; } }
        private ArrayList daysWithData; //array to check which data days are already counted

        private DateTime startDate; //included start date (later) of the interval passed to the object constructor
        public DateTime StartDate { get { return startDate;} }
        private DateTime endDate;   //included end date (earlier) of the interval passed to the object constructor
        public DateTime EndDate { get { return endDate; } }

        //structure to hold temperature and corresponding number of minimum and maximum occurences in the date intervale
        private Dictionary<int, TemperatureOccurences> tempsCount;
        public Dictionary<int, TemperatureOccurences> TempsCount { get { return tempsCount; } }

        //structure to hold hours and corresponding acummulated temperatures (from number of days defined in 'dayCounter')
        private Dictionary<int, int> accumHourlyTemps;

        //structure to hold hours and corresponding average temperatures
        private Dictionary<int, int> avgHourlyTemps;
        public Dictionary<int, int> AvgHourlyTemps{get{return avgHourlyTemps;}}

        public HistAndGraphData(string startDate, string endDate, int nHours, string local)
        {
            this.local = local;
            this.startDate = DateTime.Parse(startDate);
            this.endDate = DateTime.Parse(endDate);

            dayCounter = (int)this.endDate.Subtract(this.startDate).TotalDays;
            dayCounter++;
            daysWithData = new ArrayList(dayCounter);

            tempsCount = new Dictionary<int, TemperatureOccurences>();
            accumHourlyTemps = new Dictionary<int, int>();
            for (int hour = 0; hour < 2400; hour += 2400 / nHours*100) //hours in response data comes in hundreds format
                accumHourlyTemps.Add(hour, 0);
        }

        public bool SetDate(string setDate)
        {
            throw new NotImplementedException();
        }

        public void AddDailyTemps(int min, int max)
        {
            //FOR MINIMUM TEMPERATURES
            if (tempsCount.ContainsKey(min)) tempsCount[min].IncMinOcc(); //in case there is already this minimum temperature registered
            else tempsCount[min] = new TemperatureOccurences(1, 0);       //in case there isn't already this minimum temperature registered

            //FOR MAXIMUM TEMPERATURES
            if (tempsCount.ContainsKey(max)) tempsCount[max].IncMaxOcc(); //in case there is already this maximum temperature registered
            else tempsCount[max] = new TemperatureOccurences(0, 1);       //in case there isn't already this maximum temperature registered
        }

        internal void AddHourlyTemps(int time, int temp)
        {
            if (accumHourlyTemps.ContainsKey(time))
                accumHourlyTemps[time] += temp;
            else
                accumHourlyTemps[time] = temp;
        }

        public static HistAndGraphData Merge(HistAndGraphData[] hgData)
        {
            string local = hgData[0].Local;
            string startDate = hgData[0].StartDate.ToString(WWOClient.DATE_FORMAT);
            string endDate = hgData[hgData.Length-1].EndDate.ToString(WWOClient.DATE_FORMAT);;
            int nHours = hgData[0].accumHourlyTemps.Count;

            HistAndGraphData newHGData = new HistAndGraphData(startDate, endDate, nHours, local);

            for (int i = 0; i < hgData.Length; ++i)
                    newHGData.Append(hgData[i]);

            return newHGData;
        }

        /********************************************** PRIVATE METHODS ***************************************************/

        //Check/validate a received date is in the defined date interval (when calling the constructor)
        private bool IsValidDate(DateTime date)
        {
            return date.CompareTo(startDate) >= 0 && date.CompareTo(endDate) <= 0;
        }


        private void Append(HistAndGraphData hgData)
        {
            foreach (int temperature in hgData.tempsCount.Keys)
            {
                if (!tempsCount.ContainsKey(temperature))
                    tempsCount.Add(temperature, hgData.tempsCount[temperature]);
                else
                {
                    int min = tempsCount[temperature].MinNOccurences + hgData.tempsCount[temperature].MinNOccurences;
                    int max = tempsCount[temperature].MaxNOccurences + hgData.tempsCount[temperature].MaxNOccurences;
                    TemperatureOccurences tempOccur = new TemperatureOccurences(min, max);
                    tempsCount[temperature] = tempOccur;
                }
            }

            foreach (int hourly in hgData.accumHourlyTemps.Keys)
            {
                if (!accumHourlyTemps.ContainsKey(hourly))
                    accumHourlyTemps.Add(hourly, hgData.accumHourlyTemps[hourly]);
                else
                    accumHourlyTemps[hourly] += hgData.accumHourlyTemps[hourly];
            }
        }

        public void CalculateAvg()
        {
            avgHourlyTemps = new Dictionary<int,int>();
            foreach (int hour in accumHourlyTemps.Keys)
            {
                if (dayCounter == 0) dayCounter = 1;
                avgHourlyTemps[hour] = accumHourlyTemps[hour] / dayCounter;
            }
        }
    }
}
