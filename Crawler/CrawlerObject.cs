using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class CrawlerObject
    {
        //dicionário que vai ter como chave uma palavra e como valor associado a ela, vai ser um conjunto de links (URL) onde a mesma se encontra 
        private IDictionary<string,List<string>> dict = new Dictionary<string,List<string>>();

        public CrawlerObject(string url,int lvl) {
            Url = url;
            Level= lvl;
        }

        public int Level {
            get; set; 
        }

        public string Url {
            get;set;
        }

        public IDictionary<string, List<string>> GetDict() { 
            return dict; 
        }

        private void Merge(IDictionary<string,List<string>> list) {
            //TODO 
        }


        public void Execute() { 
            /* TODO
             *      1º Percorrer a pagina corrente, e actualizar dicionario com as palavras e respectivo link corrente
             *      2º Em simultâneo, guardar hrefs numa estrututra
             *      3º Por cada href, crio um novo Crawler filho e faço Execute() a ele
             *              4º Crio um dicionario temporário, para guardar o dicionario do crawler filho
             *      5º Por fim, chamamos o Merge Do pai, passando como argumento o dicionário do filho!
             * 
             * */

        } 


    }//close class
}
