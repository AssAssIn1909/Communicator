using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PI
{
    class Program
    {
        static Random r = new Random();

        static void Main(string[] args)
        {
            Thread thread1 = new Thread(new ThreadStart(uruchom));
            thread1.Start();
        }
        static decimal ObliczPi(ulong iloscProb)
        {
            double x, y;
            long iloscTrafien = 0;
            for (ulong i = 0; i < iloscProb; i++)
            {
                x = r.NextDouble();
                y = r.NextDouble();
                if (x * x + y * y < 1) ++iloscTrafien;

                if(i % 1000000 == 0)
                {
                    Console.Clear();
                    Decimal percent = ((decimal)i / (decimal)iloscProb)*100;
                    Console.WriteLine("{0}%", percent);
                }
                
            }
            Console.Clear();
            return 4.0M * iloscTrafien / iloscProb;
        }
        static void uruchom()
        {
            decimal pi = ObliczPi(999999999L);
            Console.WriteLine("Wynik: {0}", pi);
            Console.Read();
        }
    }
}
