using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class ConsoleUtils
    {

        static const char TEMP_MIN_CHR = '*';
        static const char TEMP_MAX_CHR = '#';
        static const char AVG_TEMP_CHR = '>';


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

            int x = histogram.TempsCount[0].MaxNOccurences;

            //histogram.TempsCount.or


            foreach (int temperature in histogram.TempsCount.Keys)
            {
                Console.Write("{0} | ", temperature);
                int max = histogram.TempsCount[temperature].MaxNOccurences;
                int min = histogram.TempsCount[temperature].MinNOccurences;
                

            
            }


            


        }

    }
}
