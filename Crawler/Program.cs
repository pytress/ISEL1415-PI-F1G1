using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Crawler
{
    class Program
    {
        const string SCHEMA_HTTP = "http://";
        const string SCHEMA_HTTPS = "https://";
        private RestClient rClient;
        private RestRequest rReq;


        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Introduza um Url e um nível de profundidade!\nE.g.--> http://www.abola.pt 2 ");
            Console.ReadLine();

            IParser<CrawlerObject> ip = new CrawlerParser();
            CrawlerObject co = ip.Parse(args);

           //1st - Guardar todas as palavras numa estrutura de dados, com os respectivos links onde elas se encontram

           //2nd - Pedir ao utilizador 1 palavra, e a aplicaçao responder à mesma com todos os links onde a palavra se encontra. (Manter aplicação
            // sempre a correr de modo que o user possa ir introduzindo várias palavras e a App ir respondendo) 

            Console.WriteLine("Press Enter to continue :)");
            Console.Read();
        }
    }
}
