using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    class CrawlerParser : IParser<CrawlerObject>
    {

        const int nr_of_params = 2; //This parser only execute with an Application that receives 2 arguments (int and URL)


        public CrawlerObject Parse(string[] args) {

            CrawlerObject co;
            if (args == null || args.Length < nr_of_params) throw new ArgumentException();
            
            else {
                co = new CrawlerObject();
                int value;
                if (int.TryParse(args[0], out value)) {
                    co.Level = value;
                    co.Url = args[1];
                }

                if (int.TryParse(args[1], out value))
                {
                    co.Level = value;
                    co.Url = args[0];
                }

                else {
                    Console.WriteLine("... else ... ERROR!!");
                    throw new ArgumentException();
                }

            }
            
            return co;
        }

        
    } //close class
}
