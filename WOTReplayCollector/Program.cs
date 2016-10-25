using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;

namespace WOTReplayCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            var titleKeywords = new List<string>();
            var descKeywords = new List<string>();

            var pages = 1;

            var options = new OptionSet()
            {
                { "t|title=", "Keyword to be search in title", v => titleKeywords.Add(v) },
                { "d|description=", "Keyword to be search in description", v => descKeywords.Add(v) },
                { "p|pages=", "Number of web pages to be collected", (int v) => pages = v }
            };

            try
            {
                options.Parse(args);

                var replays = new ReplayCollector("http://wotreplays.com/", titleKeywords.ToArray(), descKeywords.ToArray())
                    .Collect(1, pages);

                 new ReplayResultStorage(replays).Dump();
            }
            catch(OptionException e)
            {
                Console.Write("Exception when parsing input parameters: ");
                Console.WriteLine(e.Message);
            }
        }
    }
}
