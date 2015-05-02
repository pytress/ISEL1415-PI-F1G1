using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ficha1
{
    class HistAndGraphData
    {
        private class TemperatureOccurences
        {
            private int minNOccurences;
            internal int MinNOccurences { get { return minNOccurences; } }
            private int maxNOccurences;
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

        private double dayCounter;

        private DateTime startDate;
        public DateTime StartDate { get { return startDate;} }
        private DateTime endDate;
        public DateTime EndDate { get { return endDate; } }

        private Dictionary<int, TemperatureOccurences> tempsCount;
        public Dictionary<int, TemperatureOccurences> TempsCount { get { return tempsCount; } }
        //lista de horas com respetivo acumulador de temps (so no final se faz media)

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

            tempsCount = new Dictionary<int, TemperatureOccurences>();
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
            throw new NotImplementedException();
        }

        public static HistAndGraphData Merge(HistAndGraphData[] hData)
        {
            throw new NotImplementedException();
        }
    }
}
