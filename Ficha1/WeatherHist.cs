using System;
using System.Collections.Generic;

namespace Ficha1
{
    class WeatherHist
    {
        const string REQ_KEY = "-local";

        static void Main(string[] args)
        {
            //Validade args
            Dictionary<string, string> keyValuePairs = ValidateArgs(args);

            if (keyValuePairs == null)

            //Instantiate client and request data
            WWOClient client = new WWOClient(keyValuePairs);
            HistAndGraphData wData = client.GetData();

            if (wData == null)
            {
                ConsoleUtils.Pause("No data returned from server! Press ENTER to end...");
                return;
            }

            //Print Histogram
            ConsoleUtils.PrintHistogram(wData);
            ConsoleUtils.Pause();

            ConsoleUtils.PrintDailyAvg(wData);
            ConsoleUtils.Pause();
        }

        static Dictionary<string, string> ValidateArgs(string[] args)
        {
            if (args.Length == 0) //no args received or passed frmo console
            {
                Console.WriteLine("### ALERT: No arguments passed! At least a local is needed!\n");
                ConsoleUtils.Pause("A demo run will be done. Press ENTER...");

                args = new String[] { "lixo=?", "-local=Lisbon", "=", "-startdate=2015-02-06", "-enddate=2015-03-16", "xuxu=9=hy", "-asq?!" };

                Console.WriteLine("List of parameters passed:");
                for (int idx = 0; idx < args.Length; ++idx)
                    Console.WriteLine("{0,23}", args[idx]);
                
                Console.WriteLine("");
                ConsoleUtils.Pause();
            }

            IParser<Dictionary<string, string>> parser = new WWOParser();
            Dictionary<string, string> keyValuePairs = parser.Parse(args);

            IArgumentVerifier<string> av = new MandatoryArgs<string, Dictionary<string, string>>(keyValuePairs);
            if (!av.Verify(new string[] { REQ_KEY })) //TODO: allow any case (case insensitive)
            {
                Console.WriteLine("### ALERT: No required arguments received!");
                Console.WriteLine("### USAGE: weather -local=<local> [-startdate=<yyyy-MM-dd>] [-enddate=<yyyy-MM-dd>]");
                return null;
            }

            return keyValuePairs;
        }
    }
}
