using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WOTReplayCollector
{
    class ReplayCollector
    {
        public string Url { get; set; }

        public string [] Keywords { get; set; }

        public string WOTVersion { get;  set; }

        public ReplayCollector(string url, string[] keywords)
        {
            Url = url;
            Keywords = keywords;
        }

        public ReplayInfo[] Collect(int pages)
        {
            var result = new List<ReplayInfo>();
            var client = new HttpClient();
            bool endOfCollecting = false;
            int pageIndex = 1;            

            var currentUrl = Url;

            while(!endOfCollecting)
            {
                if(pages <= 0)
                {
                    endOfCollecting = true;
                    continue;
                }

                var htmlResult = client.GetAsync(currentUrl).Result;
                if(htmlResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Could not fetch HTML page (status code = {0}", htmlResult.StatusCode.ToString());
                    endOfCollecting = true;
                    continue;
                }

                var html = htmlResult.Content.ReadAsStringAsync().Result;

                List<ReplayInfo> replays = GetReplayInfoList(html);

                if (Keywords != null && Keywords.Length > 0)
                {
                    foreach (var replay in replays)
                    {
                        foreach (var keyword in Keywords)
                        {
                            if (replay.Title.ToUpper().Contains(keyword.ToUpper()) ||
                                replay.Description.ToUpper().Contains(keyword.ToUpper()))
                            {
                                result.Add(replay);
                            }
                        }
                    }
                }
                else
                {
                    result.AddRange(replays);
                }

                --pages;
                ++pageIndex;

                /* set up next page to be fetched */
                currentUrl = GetNextUrl(pageIndex);
            }          

            client.Dispose();            

            return result.ToArray();
        }

        string GetNextUrl(int index)
        {
            return String.Format("http://wotreplays.com/site/index/version/{0}/sort/uploaded_at.desc/page/{1}/", 49, index);
        }

        List<ReplayInfo> GetReplayInfoList(string html)
        {
            var client = new HttpClient();
            var result = new List<ReplayInfo>();
            var doc = new HtmlDocument();
            var descDoc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.LoadHtml(html);

            if(doc.ParseErrors == null || doc.ParseErrors.Count() > 0)
            {
                return null;
            }

            var nodes = doc.DocumentNode.SelectNodes("//ul[@class='r_list initial']//li[@class='clearfix']").ToList();

            foreach (var node in nodes)
            {

                var title = node.SelectSingleNode("//a[@class='link--pale_orange']").InnerText;
                string url = "http://wotreplays.com" + node.SelectSingleNode("//a[@class='link--pale_orange']").Attributes["href"].Value;

                var gameInfo = node.SelectSingleNode("//ul[@class='r-info_ci']");
                string tank = gameInfo.ChildNodes[1].InnerText.Substring(6);
                string map = gameInfo.ChildNodes[3].InnerText.Substring(5);
                string sent = gameInfo.ChildNodes[5].InnerText.Substring(6);
                string player = gameInfo.ChildNodes[7].InnerText.Substring(8);
                string description = string.Empty;

                // Get description from subpage
                var httpResult = client.GetAsync(url ).Result;
                if (httpResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Could not fetch HTML (description) page (status code = {0}", httpResult.StatusCode.ToString());
                }
                else
                {
                    var descHtml = httpResult.Content.ReadAsStringAsync().Result;
                    descDoc.LoadHtml(descHtml);

                    var descInfo = descDoc.DocumentNode.SelectSingleNode("//p[@id='descriptionContainer']");
                    description = descInfo.InnerText;
                }
                result.Add(
                    new ReplayInfo
                    {
                        Title = title,
                        Url = url,
                        Tank = tank,
                        Player = player,
                        Uploaded = DateTime.Parse(sent),
                        Description = description
                    });
            }

            client.Dispose();

            return result;
        }
    }
}
