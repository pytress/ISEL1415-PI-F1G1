using System.Collections.Generic;
using System.Linq;

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
}
