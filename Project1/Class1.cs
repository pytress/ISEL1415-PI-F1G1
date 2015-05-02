using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    class Class1
    {
        public static void Main(string[] args)
        {

            DateTime t = DateTime.Parse("2015-05-01");

            Console.WriteLine(t.ToString());

            t = t.AddDays(8);

            Console.WriteLine(t.ToString());
            Console.Read();

        }
    }
}
