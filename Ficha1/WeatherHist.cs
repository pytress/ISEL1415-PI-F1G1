using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class WeatherHist
    {
        const string REQ_KEY = "-local";

        static void Main(string[] args)
        {
            //Validade args
            Dictionary<string, string> keyValuePairs = ValidateArgs(args);

            //Instantiate client and request data
            WWOClient client = new WWOClient(keyValuePairs);
            HistAndGraphData wData = client.GetData();

            if (wData == null)
            {
                ConsoleUtils.Pause("No data returned! Press key to end...");
                return;
            }

            //Print Histogram
            ConsoleUtils.PrintHistrogram(wData);
            ConsoleUtils.Pause();

            ConsoleUtils.PrintDailyAvg(wData);
            ConsoleUtils.Pause();
        }

        static Dictionary<string, string> ValidateArgs(string[] args)
        {
            if (args.Length == 0) //no args passed/received
            {
                Console.WriteLine("ALERT: No arguments passed! At least a local is needed!");
                Console.WriteLine("");
                ConsoleUtils.Pause("A demo run will be done. Press a key...");
                args = new String[] { "lixo=?", "-local=Lisbon", "=", "-startdate=2012-03-05", "-enddate=2015-04-17", "xuxu=9=hy", "-asq?!" };
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
                throw new ApplicationException();     //TODO: doesn't seem to be appropriate to throw exception; better to alert user

            return keyValuePairs;
        }

        //static void PrintHistogram(WeatherData wData, string lastReqResultStatus)
        //{
        //    if (wData.IsEmpty)
        //        Console.WriteLine(" ### MSG: No data returned (Reason: {0})", lastReqResultStatus);
        //    else
        //    {
        //        Console.WriteLine(" ### MSG: Valid data obtained (not necessarilly all data requested)"); //TODO DEBUG
        //        Histogram hist = new Histogram(wData);
        //        hist.PresentResults();
        //    }
        //}
    }
}
