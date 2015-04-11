using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    interface IArgumentVerifier<T>
    {
        bool Verify(T[] keys);
    }
}
