using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class Data
    {
        public List<string> request { get; set; }
        [RestSharp.Deserializers.DeserializeAs(Name = "weather")]
        public List<WeatherData> wData { get; set; }
    }

    class Request
    {
        public string query { get; set; }
        public string type { get; set; }
    }

    class Astronomy
    {
        protected string moonrise { get; set; }
        protected string moonset { get; set; }
        protected string sunrise { get; set; }
        protected string sunset { get; set; }
    }

    class WeatherData
    {
        Dictionary<string, string> astron { get; set; }
        string date { get; set; }
        Dictionary<string, string> hourly { get; set; }
    }

    class WWOData
    {
        public Data data { get; set; }
    }
}
