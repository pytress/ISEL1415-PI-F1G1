using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.MyExceptions
{
    class ArgsException : Exception
    {
        private string msg;

        public ArgsException() {
            msg="Introduziu parâmetros inválidos";
        }

        public ArgsException(string msg) {
            this.msg = msg;
        }

        public override string ToString()
        {
            return msg;
        }

    }//class
}
