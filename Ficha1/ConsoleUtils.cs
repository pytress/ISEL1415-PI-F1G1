using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class ConsoleUtils
    {
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

            //histogram.

            


        }

    }
}
