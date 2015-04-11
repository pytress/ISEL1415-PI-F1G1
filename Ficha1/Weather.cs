using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class Weather
    {
        const string REQKEY = "local";

        static void Main(string[] args)
        {

            IParser<Dictionary<string, string>> iParser = new WWOParser();

            Dictionary<string, string> keyValuePairs = iParser.Parse(args);

            //iParser.HasMandatoryArgs(keyValuePairs, new string[] { "local" });
            /*
            if (!Verify(dic))
            throw Exception();
            */

            IArgumentVerifier<string> iav = new MandatoryArgs<string, Dictionary<string, string>>(keyValuePairs);
            if (!iav.Verify(new string[] { REQKEY })) // TODO: allow any case (case insensitive)
                throw new NotImplementedException();

            WWOClient client = new WWOClient(keyValuePairs); //implementar tipo WWORequest
            client.RequestData();
            
            WeatherData wData = client.ReturnedData();
            Histogram hist = new Histogram();
            hist.Present(Console);

            Console.WriteLine("Press ENTER to continue...");
            Console.Read();
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
