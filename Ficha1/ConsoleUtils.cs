﻿using System;
using System.Collections.Generic;
using System.Linq;

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

        public static void PrintHistogram(HistAndGraphData histogram)
        {
            Console.WriteLine("Temperature Histogram from {0} to {1} in {2}\n",
                               histogram.StartDate.ToString(WWOClient.DATE_FORMAT),
                               histogram.EndDate.ToString(WWOClient.DATE_FORMAT),
                               histogram.Local);

            List<int> temperatures = histogram.TempsCount.Keys.ToList();
            temperatures.Sort();

            Console.WriteLine(" ºC |");

            int occAcum = 0;
            foreach (int temperature in temperatures)
            {
                int max = histogram.TempsCount[temperature].MaxNOccurences;
                int min = histogram.TempsCount[temperature].MinNOccurences;

                occAcum += max;

                Console.Write("{0,2} {1}|", temperature, TEMP_MAX_CHR);
                Console.WriteLine(max.ToString().PadLeft(max + 1, TEMP_MAX_CHR));

                Console.Write("{0,2} {1}|", temperature, TEMP_MIN_CHR);
                Console.WriteLine(min.ToString().PadLeft(min + 1, TEMP_MIN_CHR));
            }

            Console.WriteLine("    |--------------------------------------");
            Console.WriteLine("                                Ocurrences");
            Console.WriteLine("Legend: {0} Maximum temperatures", TEMP_MAX_CHR);
            Console.WriteLine("        {0} Minimum temperatures\n", TEMP_MIN_CHR);
            Console.WriteLine("Total occurences: {0} | Number of days with data: {1})\n", occAcum, histogram.NDays);
        }

        public static void PrintDailyAvg(HistAndGraphData dailyAvg)
        {
            Console.WriteLine("Daily Average from {0} to {1} in {2}\n",
                               dailyAvg.StartDate.ToString(WWOClient.DATE_FORMAT),
                               dailyAvg.EndDate.ToString(WWOClient.DATE_FORMAT),
                               dailyAvg.Local);

            Dictionary<int, int> hourlyAvgTemps = dailyAvg.AvgHourlyTemps;
            List<int> hours = hourlyAvgTemps.Keys.ToList();
            hours.Sort();

            Console.WriteLine(" H |");

            foreach (int hour in hours)
            {
                int avg = (int)hourlyAvgTemps[hour];
                Console.Write("{0,2} | ", hour/100);
                Console.WriteLine(avg.ToString().PadLeft(avg, AVG_TEMP_CHR));
            }

            Console.WriteLine("   |--------------------------------------");
            Console.WriteLine("                                        ºC");
        }
    }
}
