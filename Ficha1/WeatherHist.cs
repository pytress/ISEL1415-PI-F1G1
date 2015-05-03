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


            //Print Histogram
            //PrintHistogram(wData, client.LastReqResultStatus);
            ConsoleUtils.PrintHistrogram(wData);
            ConsoleUtils.Pause();

            ConsoleUtils.PrintDailyAvg(wData);
            ConsoleUtils.Pause();
            
        }

        static Dictionary<string, string> ValidateArgs(string[] args)
        {
            //TODO: remove hardcoded args
            args = new String[] { "lixo=?", "-local=Lisbon", "=", "-startdate=2015-04-01", "-enddate=2015-04-15", "xuxu=9=hy", "-asq?!" }; //TODO DEBUG: for test purposes

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
