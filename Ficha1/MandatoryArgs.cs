using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ficha1
{
    class MandatoryArgs<T,U> : IArgumentVerifier<T> where U : IDictionary<T,T>
    {
        private U argsToVerify;
    
        public MandatoryArgs(U args)
        {
            argsToVerify = args;
        }

        public bool Verify(T[] mandatoryKeys)
        {
            foreach (T key in mandatoryKeys)
                if (!argsToVerify.ContainsKey(key))
                    return false;

            return true;
        }
    }

    /*
    class WWOMandatoryArgs<T> : IArgumentVerifier<T>
    {
        private Dictionary<T,T> argsToVerify;
        private T susd;

        public WWOMandatoryArgs(Dictionary<T,T> args)
        {
            argsToVerify = args;
        }

        public bool Verify(T[] keys)
        {
            throw new NotImplementedException();
        }
    }
    */
}

