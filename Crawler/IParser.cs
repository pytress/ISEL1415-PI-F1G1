using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    interface IParser<T>
    {
        T Parse(string[] args);
    }
}
