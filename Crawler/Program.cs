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
        
            Console.WriteLine("Introduza um Url e um nível de profundidade \n E.g.--> http://www.abola.pt 2 ");
            string arguments = Console.ReadLine();

            IParser<CrawlerObject> parser = new CrawlerParser();
            CrawlerObject crawler = parser.Parse(arguments);

            crawler.Execute();
          
            Console.WriteLine("Indique a palavra a pesquisar");
            String word = Console.ReadLine();
            //FindWord ... Get the word that it could be in the dictionary

            Console.WriteLine("Press any key to exit :)");
            Console.ReadKey();
        }
    }
}
