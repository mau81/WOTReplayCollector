using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WOTReplayCollector
{
    class ReplayResultStorage
    {
        ReplayInfo[] Replays { get; set; }

        public ReplayResultStorage(ReplayInfo[] replays)
        {
            Replays = replays;
        }

        public void Dump()
        {
            var json = JsonConvert.SerializeObject(Replays);
            File.WriteAllText(@".\dump.txt", json);
        }
    }
}
