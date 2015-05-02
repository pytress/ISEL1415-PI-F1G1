using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class HistData //TODO: verificar se há e resolver problema da possivel inconsistencia entre intervalo de dias usado p/ criação do objeto e os que realmente estão inseridos
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

        private string startDate, endDate;
        private Dictionary<int, TemperatureOccurences> tempsCount;

        public HistData(string startDate, string endDate) //TODO: verificar se também é necessário verificar se start < end e consequentemente se deverão passar a tipo DateTime
        {
            this.startDate = startDate;
            this.endDate = endDate;
            tempsCount = new Dictionary<int, TemperatureOccurences>();
        }

        public void AddTemps(int min, int max)
        {
            //FOR MINIMUM TEMPERATURES
            if (tempsCount.ContainsKey(min)) tempsCount[min].IncMinOcc(); //in case there is already this minimum temperature registered
            else tempsCount[min] = new TemperatureOccurences(1, 0);       //in case there isn't already this minimum temperature registered

            //FOR MAXIMUM TEMPERATURES
            if (tempsCount.ContainsKey(max)) tempsCount[max].IncMaxOcc(); //in case there is already this maximum temperature registered
            else tempsCount[max] = new TemperatureOccurences(0, 1);       //in case there isn't already this maximum temperature registered
        }
    }
}
