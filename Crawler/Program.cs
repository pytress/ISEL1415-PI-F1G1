using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Crawler.MyExceptions;

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
            try
            {
                IParser<CrawlerObject> parser = new CrawlerParser();
                CrawlerObject crawler = parser.Parse(arguments);

                crawler.Execute(); // :O  Hard work it's here!!! :'(

                Console.WriteLine("Indique a palavra a pesquisar");
                String word = Console.ReadLine();
                crawler.FindWord(word);// ... Get the word that it could be in the dictionary              
            }

            catch (ArgsException excep)
            {
                Console.WriteLine(excep.ToString());
            }

            /* Other Catch Block to treat the exceptions Goes Here...  :D   :D 

                           ..................... */

            finally {
                Console.WriteLine("Press any key to exit :)");
                Console.ReadKey();
            }

        }//MAIN
    }//class
}