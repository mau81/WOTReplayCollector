using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTReplayCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            var replays = new ReplayCollector("http://wotreplays.com/", new string[] { })
                .Collect(1);

            foreach(var replay in replays)
            {
                Console.WriteLine("{0}\n\n\n", replay);
            }

            return;
        }
    }
}
