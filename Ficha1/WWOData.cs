using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class Request
    {
        protected string query { get; set; }
        protected string type { get; set; }
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
        Request req { get; set; }
        [RestSharp.Deserializers.DeserializeAs(Name = "weather")]
        WeatherData wData { get; set; }
    }
}
