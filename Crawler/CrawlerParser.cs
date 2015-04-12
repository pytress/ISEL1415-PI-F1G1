using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    class CrawlerParser : IParser<CrawlerObject>
    {
        
        public CrawlerParser() { 
            
        }

        public CrawlerObject Parse(string[] args) {
            CrawlerObject co;

            if (args == null) throw new ApplicationException();
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
                    throw new ArgumentException();
                }

            }
            
            return co;
        }
        
    } //close class
}
