using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class MandatoryArgs<T,U> : IArgumentVerifier<T>
    {
        public MandatoryArgs(U args)
        {
        }

        public bool Verify(T[] keys)
        {
            throw new NotImplementedException();
        }
    }
}

