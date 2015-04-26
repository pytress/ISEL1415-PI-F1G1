using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    class CrawlerParser : IParser<CrawlerObject>
    {

        const int NR_OF_PARAMS = 2; //This parser only execute with an Application that receives 2 arguments (int and URL)


        public CrawlerObject Parse(string arguments) {

            CrawlerObject co;
            char[] delimiter = { ' ' };
            String[] args = arguments.Split(delimiter);

            if (args == null || args.Length < NR_OF_PARAMS) throw new ArgumentException();
            
            else {
                string url;
                int value;
                if (int.TryParse(args[0], out value)) {
                    url = args[1];
                    if (value < 0) throw new ArgumentException(); //o nivel de profundidade introduzido é menor que 0
                }

                else if (int.TryParse(args[1], out value))
                {
                    url = args[0];
                    if (value < 0) throw new ArgumentException(); //o nivel de profundidade introduzido é menor que 0
                }
                    //qualquer coisa
                else {
                    // TODO --> what should i do?
                    Console.WriteLine("... else ... ERROR!!");
                    throw new ArgumentException();
                }

                co = new CrawlerObject(url, value);
            }
            
            return co;
        }

        
    } //close class
}
