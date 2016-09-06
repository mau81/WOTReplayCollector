using System;

namespace WOTReplayCollector
{
    public class ReplayInfo
    {
        public string Title { get; set; }

        public string Tank { get; set; }

        public string Description { get; set; }

        public string Player { get; set; }

        public string VersionString { get; set; }

        public string Url { get; set; }

        public DateTime Uploaded { get; set; }

        public override string ToString()
        {
            return String.Format("Title: {0}\nPlayer: {1}\nTank: {2}\n Uploaded: {3}\n URL: \n Description: {5}\n", 
                Title, Player, Tank, Uploaded.ToShortTimeString(), Url, Description);
        }
    }
}