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

        //public int Level { get; set; }
        //public string Url { get; set; }
        private int level;
        private string url;
        private static List<string> visitedUrls = new List<string>();

        private RestClient client;
        private RestRequest req;
        private const string CONTENT_TYPE_VALUE = "text/html"; 


        public CrawlerObject(string url,int lvl) {
            this.url = url;
            level= lvl;
            client = new RestClient(this.url);
            client.FollowRedirects = false;
            req = new RestRequest(Method.GET);
            req.AddHeader("Accept", "text/html");
        } //constructor

        
        private void Merge(IDictionary<string,List<string>> childDict) {

            foreach (string s in childDict.Keys)
            {
                if (!dict.Keys.Contains(s))
                    dict[s] = childDict[s];
                else
                {
                    foreach (string link in childDict[s])
                    {
                        if (!dict[s].Contains(link))
                            dict[s].Add(link);
                    }
                }
            }

        }


        public IDictionary<string, List<string>> Execute()
        {
            RestResponse resp = (RestResponse)client.Execute(req);
            //Validate response content
            if (resp.Content == "" ||
                !(resp.ContentType.Contains(CONTENT_TYPE_VALUE)) ||
                resp.StatusCode != HttpStatusCode.OK)
                return dict;

            // Guardo numa string o conteúdo da resposta HTTP
            string resp_content = resp.Content;

            //Agora quero só o body da resposta HTTP
            int first = resp_content.IndexOf("<body");
            int length = resp_content.IndexOf("</body>") - first + "</body>".Length;

            string body;
            if (first <= 0)
                body = "";
            else
                body = resp_content.Substring(first, length);


            //Index words
            FillDictWithWords(body);

            if (level > 0)
            {
                //Index links
                FillListWithRefs(body);

                //paralel for, para executar todos os pedidos referentes às strings presentes na lista hrefs! 
                Parallel.For(0, hrefs.Count, i =>
                //for (int i = 0; i < hrefs.Count; ++i)
                {
                    CrawlerObject crawler_temp = new CrawlerObject(hrefs[i], level - 1);
                    IDictionary<string, List<string>> dict_temp = crawler_temp.Execute();
                    Merge(dict_temp);
                });
            }

            //Progress bar    
            Console.Write(".");
            
            return dict;

        } //close method Execute()



        private void FillDictWithWords(string body) {
            Match m = Regex.Match(body, @"\b[a-z]*\b", RegexOptions.IgnoreCase);

            while (m.Success)
            {
                if (m.Value.Length > 0 && !dict.Keys.Contains(m.Value))
                    dict[m.Value] = new List<string>() { url };

                m = m.NextMatch();
            }
        }

        private void FillListWithRefs(string body) {
            if (body == null || body.Length < 1) return;

            Match m = Regex.Match(body, "href=\"([^\"]*)\"", RegexOptions.IgnoreCase);

            while (m.Success)
            {
                string link = m.Groups[1].Value;

                //If uri is relative, transform it in absolute
                if (Uri.IsWellFormedUriString(link, UriKind.Relative))
                    link = url + "/" + link;
                
                if(Uri.IsWellFormedUriString(link, UriKind.Absolute) &&
                    !hrefs.Contains(link) &&
                    link.Length > 0 &&
                    !visitedUrls.Contains(link)) {
                    hrefs.Add(link);
                    visitedUrls.Add(link);
                }
                m = m.NextMatch();
            }

            
        } //close method FillListWithRefs(string body)

        //TODO This method must goes to another class
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
                Console.WriteLine("Palavra não encontrada");
            }
        }//method FindWord


        public int CountWords()
        {
            return dict.Count;
        }

    }//close class
}
