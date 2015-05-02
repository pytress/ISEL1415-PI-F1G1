using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using System.Text.RegularExpressions;

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
        private const string CONTENT_TYPE_VALUE = "text/html"; 


        public CrawlerObject(string url,int lvl) {
            url = AdjustURL(url);
            Url = url;
            Level= lvl;            
            client = new RestClient(url);
            req = new RestRequest(Method.GET);
            req.AddHeader("Accept", "text/html");
        } //constructor

        
        private string AdjustURL(string url)
        {
            string http="http://";

            if (!url.Contains("https") && !url.Contains("http")) { url = string.Concat(http, url); }           
            return url;
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

        private void Merge(IDictionary<string,List<string>> dict) {
            //TODO uniar os dois dicionários! Cuidado pk mesmo que as chaves sejam iguais, ainda tenho que fazer union das listas (valores da key)

            Console.WriteLine("I'm in method Merge()");
        }


        public IDictionary<string, List<string>> Execute()
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
            if ( !(resp.ContentType.Contains(CONTENT_TYPE_VALUE)) || resp.StatusCode != HttpStatusCode.OK) throw new ApplicationException();

            // Guardo numa string o conteúdo da resposta HTTP
            string resp_content = resp.Content;

            //Agora quero só o body da resposta HTTP
            int first = resp_content.IndexOf("<body>");
            int length = resp_content.IndexOf("</body>") - first + "</body>".Length;

            string body = resp_content.Substring(first, length);


            FillListWithRefs(body);
            FillDictWithWords(body);

            if (Level > 0)
            {
                //paralel for, para executar todos os pedidos referentes às strings presentes na lista hrefs! 
                for (int i = 0; i < hrefs.Count; ++i)
                {
                    CrawlerObject crawler_temp = new CrawlerObject(hrefs[i], Level - 1);
                    IDictionary<string, List<string>> dict_temp = crawler_temp.Execute();
                    Merge(dict_temp);
                }
            }
                // work work work... ... ...

                Console.WriteLine(" I'm the return of method Execute() :)))))))))))) ");
                return dict;

        } //close method Execute()



        private void FillDictWithWords(string body) { 
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' , '<' , '>' , '=', '\r', '\n'};

            string[] words = body.Split(delimiterChars);

            foreach (string word in words)
            {
                if (dict.ContainsKey(word) == false) {
                    dict.Add(word, new List<string>() { this.Url});
                }
            }
        }



        private void FillListWithRefs(string body) {
            Match m = Regex.Match(body, @"<a.*?href=\""(.*?)\""", RegexOptions.Singleline);

            while (m.Success)
            {
                string link = m.Groups[1].Value;
                if(!hrefs.Contains(link)) {
                    hrefs.Add(link); /*ATTENTION: link could be non-http. E.G. --> mailto:xxxxx@yyyyyy.pt  */
                }
                m = m.NextMatch();
            }

            
        } //close method FillListWithRefs(string body)

        public void FindWord(string word)
        {
            if (dict.ContainsKey(word))
            {
                List<string> links = dict[word];
                foreach (string link in links) {
                    Console.WriteLine(link + " ");
                }
            }
            else {
                Console.WriteLine("The word was not found!");
            }
        }//method FindWord

    }//close class
}
