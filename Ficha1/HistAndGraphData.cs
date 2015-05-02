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
            private int minNOccurences; //number of a minimum temperature occurences
            internal int MinNOccurences { get { return minNOccurences; } }
            private int maxNOccurences; //number of a maximum temperature occurences
            internal int MaxNOccurences { get { return maxNOccurences; } }

            internal TemperatureOccurences()
            {
                minNOccurences = maxNOccurences = 0;
            }
            internal TemperatureOccurences(int tMin, int tMax)
            {
                minNOccurences = tMin; maxNOccurences = tMax;
            }

            internal void IncMinOcc() { ++minNOccurences; }
            internal void IncMaxOcc() { ++maxNOccurences; }
        }

        private double dayCounter; //counts the number of data days the object contains
        private ArrayList daysWithData; //array to check which data days are already counted

        private DateTime startDate; //included start date (later) of the interval passed to the object constructor
        public DateTime StartDate { get { return startDate;} }
        private DateTime endDate;   //included end date (earlier) of the interval passed to the object constructor
        public DateTime EndDate { get { return endDate; } }

        //TODO: renomear para dailyTempsCount
        private Dictionary<int, TemperatureOccurences> tempsCount; //structure to hold temperature and corresponding number of minimum and maximum occurences in the date intervale
        public Dictionary<int, TemperatureOccurences> TempsCount { get { return tempsCount; } }

        private Dictionary<int, int> accumHourlyTemps; //structure to hold hours and corresponding acummulated temperatures (from number of days defined in 'dayCounter')
        //public Dictionary<int, int> AccumHourlyTemps { get { return accumHourlyTemps; } }

        public HistAndGraphData(string startDate, string endDate)
        {
            dayCounter = 0;

            DateTime start = DateTime.Parse(startDate);
            DateTime end = DateTime.Parse(startDate);

            if (start.CompareTo(end) <= 0) {
                this.startDate = start;
                this.endDate = end;
            } else {
                this.startDate = end;
                this.endDate = start;
            }

            daysWithData = new ArrayList((int)this.endDate.Subtract(this.startDate).TotalDays);

            tempsCount = new Dictionary<int, TemperatureOccurences>();
            accumHourlyTemps = new Dictionary<int, int>();
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

        public static HistAndGraphData Merge(HistAndGraphData[] hData)
        {
            //throw new NotImplementedException();
            return hData[0];

        }

        //returns a stucture with the same hours as the ones in 'accumHourlyTemps' and respective average temperature
        public Dictionary<int, double> GetHourlyAvgTemps()
        {
            if (dayCounter == 0)
                throw new InvalidOperationException("None data days present!");
            //if (TimeSpan.Compare(TimeSpan.FromDays(dayCounter), endDate.Subtract(startDate)) != 0)
            if (endDate.Subtract(startDate).TotalDays != dayCounter)
                throw new InvalidOperationException("Incomplete or inconsistent data!");

            Dictionary<int, double> AvgHourlyTemps = new Dictionary<int, double>();

            foreach (KeyValuePair<int, int> kVPair in accumHourlyTemps)
            {
                AvgHourlyTemps[kVPair.Key] = (double)kVPair.Value / dayCounter;
            }

            return AvgHourlyTemps;
        }

        /********************************************** PRIVATE METHODS ***************************************************/

        //Check/validade a received date is in the defined date interval (when calling the constructor)
        private bool IsValidDate(DateTime date)
        {
            return date.CompareTo(startDate) >= 0 && date.CompareTo(endDate) <= 0;
        }
    }
}
