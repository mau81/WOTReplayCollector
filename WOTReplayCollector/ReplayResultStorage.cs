using System;
using System.Collections.Generic;
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
            foreach(var replay in Replays)
            {
                var json = new DataContractJsonSerializer(typeof(ReplayInfo));
            }
        }
    }
}
