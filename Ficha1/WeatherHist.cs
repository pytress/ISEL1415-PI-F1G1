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
            //client.RequestData();//TODO remove this
            client.RequestAsyncData();
            WeatherData wData = client.ReturnedData;

            //Print Histogram
            PrintHistogram(wData, client.LastReqResultStatus);

            ConsoleUtils.Pause();
        }

        static Dictionary<string, string> ValidateArgs(string[] args)
        {
            //TODO: remove hardcoded args
            args = new String[] { "lixo=?", "-local=Lisbon", "=", "-enddate=2015-04-22", "xuxu=9=hy", "-startdate=2015-03-22", "-asq?!" }; //TODO DEBUG: for test purposes

            IParser<Dictionary<string, string>> parser = new WWOParser();
            Dictionary<string, string> keyValuePairs = parser.Parse(args);

            IArgumentVerifier<string> av = new MandatoryArgs<string, Dictionary<string, string>>(keyValuePairs);
            if (!av.Verify(new string[] { REQ_KEY })) //TODO: allow any case (case insensitive)
                throw new ApplicationException();     //TODO: doesn't seem to be appropriate to throw exception; better to alert user

            return keyValuePairs;
        }

        static void PrintHistogram(WeatherData wData, string lastReqResultStatus)
        {
            if (wData.IsEmpty)
                Console.WriteLine(" ### MSG: No data returned (Reason: {0})", lastReqResultStatus);
            else
            {
                Console.WriteLine(" ### MSG: Valid data obtained (not necessarilly all data requested)"); //TODO DEBUG
                Histogram hist = new Histogram(wData);
                hist.PresentResults();
            }
        }



    }
}
/*
Pedidos HTTP para apresentar informacao de temperaturas de uma localidade num intervalo de tempo.
Apresentar num histograma das temperaturas maximas e minimas entre essas datas e temperaturas medias para cada um dos intervalos selecionados (parametro tp da API).
Recebe da linha de comando: nome da localidade (unico obrigatorio) e intervalo de datas. Se data não for fornecida assume-se data atual. Exemplo:
    weather.exe -local Lisboa -startdate 2015-03-25 -enddate 2015-03-30
O output deve indicar local e intervalo de datas a que o histograma se refere.
Usar framework RestSharp para realizar pedidos HTTP. Alinhamento do texto na consola pode ser realizado com as funcoes de padding da String.
Notar que as respostas API apenas suportam pedidos para intervalos maximos de 30 dias. Se for solicitado intervalo superior terão de ser realizados varios pedidos.
*/
