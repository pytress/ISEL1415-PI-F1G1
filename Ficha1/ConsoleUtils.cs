using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class ConsoleUtils
    {

        const char TEMP_MIN_CHR = '*';
        const char TEMP_MAX_CHR = '#';
        const char AVG_TEMP_CHR = '>';


        public static void Pause()
        {
            Console.WriteLine("Press ENTER to continue...");
            Console.Read();
            Console.Read();
        }

        public static void Pause(String msg)
        {
            Console.WriteLine(msg);
            Console.Read();
            Console.Read();
        }

        public static void PrintHistrogram(HistAndGraphData histogram)
        {

            Console.WriteLine("Temperature Histogram from {0} to {1}\n",
                               histogram.StartDate.ToString(WWOClient.DATE_FORMAT),
                               histogram.EndDate.ToString(WWOClient.DATE_FORMAT));

            List<int> temperatures = histogram.TempsCount.Keys.ToList();
            temperatures.Sort();

            Console.Write("ºC |");

            foreach (int temperature in temperatures)
            {
                int max = histogram.TempsCount[temperature].MaxNOccurences;
                int min = histogram.TempsCount[temperature].MinNOccurences;

                Console.Write("{0} | ", temperature);
                Console.WriteLine(max.ToString().PadLeft(max, TEMP_MAX_CHR));

                Console.Write("{0} | ", temperature);
                Console.WriteLine(min.ToString().PadLeft(min, TEMP_MIN_CHR));
            
            }

            Console.Write("   |--------------------------------------");
            Console.Write("                               Ocurrências");


        }

    }
}
