using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello world by HF\n");
            Console.WriteLine("Hello world by RG\n");

            Console.WriteLine("Press ENTER to conntinue...");
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
