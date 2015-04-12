using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class Request
    {
        public string query { get; set; }
        public string type { get; set; }
    }

    class Astronomy
    {
        protected string moonrise { get; set; }
    }

    class WeatherData
    {
        Astronomy astron { get; set; }
        string date { get; set; }
    }

    class WWOData
    {
        public Request req { get; set; }
        [RestSharp.Deserializers.DeserializeAs(Name = "weather")]
        public WeatherData wData { get; set; }
    }
}
