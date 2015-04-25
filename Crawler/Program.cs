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
        
            Console.WriteLine("Introduza um Url \n E.g.--> http://www.abola.pt ");
            string url = Console.ReadLine();
            Console.WriteLine("Indique o nível de profundidade \n E.g. --> 2");
            int lvl = Convert.ToInt32(Console.ReadLine());

            ICrawlerObject crawler = new CrawlerObject();

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
