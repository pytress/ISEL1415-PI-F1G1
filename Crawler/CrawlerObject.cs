using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Crawler
{
    class CrawlerObject
    {
        //dicionário que vai ter como chave uma palavra e como valor associado a ela, vai ser um conjunto de links (URL) onde a mesma se encontra 
        private IDictionary<string,List<string>> dict = new Dictionary<string,List<string>>();

        //lista de Url's que terão que ser visitados
        private List<string> hrefs = new List<string>();

        private RestClient client;
        private RestRequest req;
        private RestResponse resp;

        public CrawlerObject(string url,int lvl) {
            Url = url;
            Level= lvl;
            client = new RestClient(url);
            req = new RestRequest(Method.GET);
            req.AddHeader("Accept", "text/html");
        } //constructor

        public int Level {
            get; set; 
        }

        public string Url {
            get;set;
        }

        public IDictionary<string, List<string>> GetDict() { 
            return dict; 
        }

        private void Merge(IDictionary<string,List<string>> dict) {
            //TODO 
        }


        public void Execute()
        {
            /* TODO
             *      1º Percorrer a pagina corrente, e actualizar dicionario com as palavras e respectivo link corrente (Cuidado
             *      porque só interessa os href do BODY e mesmo assim não sei se são todos!)
             *      2º Em simultâneo, guardar hrefs numa estrututra
             *      3º Por cada href, crio um novo Crawler filho e faço Execute() a ele
             *              4º Crio um dicionario temporário, para guardar o dicionario do crawler filho
             *      5º Por fim, chamamos o Merge Do pai, passando como argumento o dicionário do filho!
             * 
             * */




            /* isto tudo dentro do ciclo q percorre os Hrefs a visitar
            
            crawler1

            crawler 2 = new Crawler ("/sporting",--level);
            crawler2.Execute()
            Dictionary temp =  crawler2.GetDict();

            crawler1.Merge(temp);   */

            resp = (RestResponse)client.Execute(req);
            if (resp.ContentType!="text/html") throw new ApplicationException;

            // work work work... ... ...

    


        } 


    }//close class
}
