using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class CrawlerDB
    {
        //dicionário que vai ter como chave uma palavra e como valor associado a ela, vai ser um conjunto de links (URL) onde a mesma se encontra 
        private IDictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

        public IDictionary<string, List<string>> GetDatabase() {
            return dict;
        }

        /*
        public List<string> FindWord(string word) {
            if (dict.Contains ? ) { return dict[word]; }
        } */

        public void AddListToWord(string word, List<string> hrefs) {
            dict.Add(word, hrefs); // It's NOT correct!!!!!!!!!
        }

    }//close class
}
