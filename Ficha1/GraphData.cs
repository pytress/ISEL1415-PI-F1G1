using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class GraphDataChunk //TODO: resolver problema da possivel inconsistencia entre o intervalo de dias usado para criação do objeto e os que realmente estão inseridos
    {
        private class HourlyTemperature
        {
            private int hour;
            internal int Hour { get { return hour; } }
            private int temperature;
            internal int Temperature { get { return temperature; } }

            internal HourlyTemperature(int hour, int temp)
            {
                this.hour = hour; temperature = temp;
            }
        }

        private DateTime startDate, endDate, selectedDate;
        public double NumberOfDays { get { return endDate.Subtract(startDate).TotalDays; } }

        private Dictionary<DateTime, List<HourlyTemperature>> hourlyTempsByDay; //KEY: day; VALUE: list of (hour, temperature)

        public GraphDataChunk(string startDate, string endDate)
        {
            DateTime start = DateTime.Parse(startDate);
            DateTime end = DateTime.Parse(startDate);

            if (start.CompareTo(end) <= 0)
            {
                this.startDate = start;
                this.endDate = end;
            }
            else
            {
                this.startDate = end;
                this.endDate = start;
            }

            selectedDate = DateTime.MaxValue;
            hourlyTempsByDay = new Dictionary<DateTime, List<HourlyTemperature>>();
        }

        public bool SetDate(string day)
        {
            DateTime date = DateTime.Parse(day);

            if (!IsValidDate(date))
                return false;

            selectedDate = date;
            if (!hourlyTempsByDay.ContainsKey(selectedDate))
                hourlyTempsByDay[selectedDate] = new List<HourlyTemperature>();

            return true;
        }

        public void AddHourlyTemps(int hour, int temp)
        {
            if (selectedDate == DateTime.MaxValue)
                throw new InvalidOperationException("Day was not set!");

            HourlyTemperature newHTemp = new HourlyTemperature(hour, temp);

            if (hourlyTempsByDay[selectedDate].Exists(hTemp => hTemp.Hour == hour))
                throw new InvalidOperationException("Insertion of an already existent hour for this day is not allowed!");

            hourlyTempsByDay[selectedDate].Add(newHTemp);
        }

        public Dictionary<int, int> AccumHourlyTemps()
        {
            if (hourlyTempsByDay.Count == 0) return null;

            Dictionary<int, int> accum = new Dictionary<int, int>();

            var anElem = hourlyTempsByDay.First(); //obtain one element from the hourly temperatures by day
            DateTime countedDate = anElem.Key; //saves key reference to avoid counting this elem twice
            List<HourlyTemperature> hTList = anElem.Value; //obtain one list of hourly temperatures to initiate sums and also to serve as hours reference

            hTList.ForEach(hTemp => accum[hTemp.Hour] = hTemp.Temperature); //initializes sums to grant key-hour exists

            foreach (var elem in hourlyTempsByDay)
            {
                if (elem.Value.Count != accum.Count) throw new InvalidOperationException("Not consistent hourly temperatures info!");
                if (elem.Key != countedDate)
                    elem.Value.ForEach(hTemp => {
                        if (!accum.ContainsKey(hTemp.Hour)) throw new InvalidOperationException("Not consistent hourly temperatures info!");
                        accum[hTemp.Hour] += hTemp.Temperature;
                    });
            }

            return accum;
        }

        private bool IsValidDate(DateTime day)
        {
            return (day.CompareTo(startDate) < 0 || day.CompareTo(endDate) > 0) ? false : true;
        }
    }

    class GraphData
    {
        string startDate, endDate;
        double nReferencedDays;
        private Dictionary<int, int> hourlyAvgTemp;

        public GraphData(string startDate, string endDate) //TODO: verificar se também é necessário verificar se start < end e consequentemente se deverão passar a tipo DateTime
        {
            this.startDate = startDate;
            this.endDate = endDate;
            nReferencedDays = 0;
            hourlyAvgTemp = new Dictionary<int, int>();
        }

        public void AddChunk(GraphDataChunk dataChunk)
        {
            AddPartialAccum(dataChunk.AccumHourlyTemps(), dataChunk.NumberOfDays);
        }

        public void AddPartialAccum(Dictionary<int, int> part, double nDays)
        {
            if (part != null) //if there is something to add
            {
                if (hourlyAvgTemp.Count == 0) //if this is the first data entered
                    hourlyAvgTemp = part;
                else
                {
                    if (part.Count != hourlyAvgTemp.Count || !part.Keys.All(hour => hourlyAvgTemp.ContainsKey(hour))) //check if the day hours are the same
                        throw new InvalidOperationException("Hourly temperatures to add does not correspond to existing ones!");
                    foreach (KeyValuePair<int, int> hTemp in part)
                        hourlyAvgTemp[hTemp.Key] += hTemp.Value;
                }

                nReferencedDays += nDays;
            }
        }
    }
}
