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

            //internal void AddMaxOcc(TemperatureOccurences temperatureOccurences)
            //{
            //    throw new NotImplementedException();
            //}

            //internal void AddMinOcc(int p)
            //{
            //    throw new NotImplementedException();
            //}
        }

        private int dayCounter; //counts the number of data days the object contains
        public int NDays { get { return dayCounter; } }
        private ArrayList daysWithData; //array to check which data days are already counted

        private DateTime startDate; //included start date (later) of the interval passed to the object constructor
        public DateTime StartDate { get { return startDate;} }
        private DateTime endDate;   //included end date (earlier) of the interval passed to the object constructor
        public DateTime EndDate { get { return endDate; } }

        //TODO: renomear para dailyTempsCount
        //structure to hold temperature and corresponding number of minimum and maximum occurences in the date intervale
        private Dictionary<int, TemperatureOccurences> tempsCount;
        public Dictionary<int, TemperatureOccurences> TempsCount { get { return tempsCount; } }

        //structure to hold hours and corresponding acummulated temperatures (from number of days defined in 'dayCounter')
        private Dictionary<int, int> accumHourlyTemps;

        //structure to hold hours and corresponding average temperatures
        private Dictionary<int, int> avgHourlyTemps;
        public Dictionary<int, int> AvgHourlyTemps{get{return avgHourlyTemps;}}

        public HistAndGraphData(string startDate, string endDate, int nHours) //TODO: podemos verificar correção de dados porque sabemos quantos dias e quantas horas
        {
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
            //TODO delete this
            //if (!accumHourlyTemps.ContainsKey(time)) throw new InvalidOperationException("Day hourly time not expected!");
            //accumHourlyTemps[time] += temp;
            
            if (accumHourlyTemps.ContainsKey(time))
                accumHourlyTemps[time] += temp;
            else
                accumHourlyTemps[time] = temp;
        }

        public static HistAndGraphData Merge(HistAndGraphData[] hgData)
        {
            string startDate = hgData[0].StartDate.ToString(WWOClient.DATE_FORMAT);
            string endDate = hgData[hgData.Length-1].EndDate.ToString(WWOClient.DATE_FORMAT);;
            int nHours = hgData[0].accumHourlyTemps.Count;

            HistAndGraphData newHGData = new HistAndGraphData(startDate, endDate, nHours);

            for (int i = 0; i < hgData.Length; ++i)
                    newHGData.Append(hgData[i]);

            return newHGData;
        }

        ////returns a stucture with the same hours as the ones in 'accumHourlyTemps' and respective average temperature
        //public Dictionary<int, double> GetHourlyAvgTemps()
        //{
        //    if (dayCounter == 0)
        //        throw new InvalidOperationException("None data days present!");
        //    //if (TimeSpan.Compare(TimeSpan.FromDays(dayCounter), endDate.Subtract(startDate)) != 0)
        //    if (endDate.Subtract(startDate).TotalDays != dayCounter)
        //        throw new InvalidOperationException("Incomplete or inconsistent data!");

        //    Dictionary<int, double> AvgHourlyTemps = new Dictionary<int, double>();

        //    foreach (KeyValuePair<int, int> kVPair in accumHourlyTemps)
        //    {
        //        AvgHourlyTemps[kVPair.Key] = (double)kVPair.Value / dayCounter;
        //    }

        //    return AvgHourlyTemps;
        //}

        /********************************************** PRIVATE METHODS ***************************************************/

        //Check/validade a received date is in the defined date interval (when calling the constructor)
        private bool IsValidDate(DateTime date)
        {
            return date.CompareTo(startDate) >= 0 && date.CompareTo(endDate) <= 0;
        }


        private void Append(HistAndGraphData hgData)
        {
            //TODO delete this
            //foreach (KeyValuePair<int, TemperatureOccurences> kVPair in hgData.TempsCount)
            //    if (tempsCount.Count == 0)
            //        tempsCount.Add(kVPair.Key, kVPair.Value);
            //    else
            //    {
            //        tempsCount[kVPair.Key].AddMinOcc(kVPair.Value.MinNOccurences);
            //        tempsCount[kVPair.Key].AddMaxOcc(kVPair.Value);
            //    }


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


            //foreach (KeyValuePair<int, int> kVPair in hgData.accumHourlyTemps) //acesso ao campo privado do parametro!?
            //{
            //    ++dayCounter;
            //    if (accumHourlyTemps.Count == 0)
            //        accumHourlyTemps.Add(kVPair.Key, kVPair.Value);
            //    else
            //        accumHourlyTemps[kVPair.Key] += kVPair.Value;
            //}

            foreach (int hourly in hgData.accumHourlyTemps.Keys)    //TODO acesso ao campo privado do parametro!?
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
