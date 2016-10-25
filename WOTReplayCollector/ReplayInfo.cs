using System;
using System.Runtime.Serialization;

namespace WOTReplayCollector
{
    [DataContract]
    public class ReplayInfo
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Tank { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Player { get; set; }

        [DataMember]
        public string VersionString { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public DateTime Uploaded { get; set; }

        [DataMember]
        public string[] Keywords { get; set; }

        public override string ToString()
        {
            return String.Format("Title: {0}\nPlayer: {1}\nTank: {2}\n Uploaded: {3}\n URL: \n Description: {5}\n", 
                Title, Player, Tank, Uploaded.ToShortTimeString(), Url, Description);
        }
    }
}