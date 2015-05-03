using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Crawler.MyExceptions;
using System.Diagnostics;

namespace Crawler
{
    class Program
    {
        const string SCHEMA_HTTP = "http://";
        const string SCHEMA_HTTPS = "https://";

        static void Main(string[] args)
        {
            Console.Clear();
        
            Console.WriteLine("Introduza um Url e um nível de profundidade \n E.g.--> http://www.isel.pt 2 \n");
            Console.Write("> ");
            string arguments = Console.ReadLine();
            try
            {
                IParser<CrawlerObject> parser = new CrawlerParser();
                CrawlerObject crawler = parser.Parse(arguments);

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                Console.Write("\nCrawling ...");
                crawler.Execute(); // :O  Hard work it's here!!! 
                
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

                Console.WriteLine("\n\nForam indexadas {0} palavras em {1} segundos", crawler.CountWords(), elapsedTime);
                Console.WriteLine("Indique a palavra a pesquisar (escreva \"sair\" para terminar)\n");
                Console.Write("> ");
                String word = Console.ReadLine();
                while (word != "sair")
                {
                    Console.WriteLine();
                    crawler.FindWord(word);// ... Get the word that it could be in the dictionary              
                    Console.Write("\n> ");
                    word = Console.ReadLine();
                }

            }

            catch (ArgsException excep)
            {
                Console.WriteLine(excep.ToString());
            }


        }//MAIN
    }//class
}