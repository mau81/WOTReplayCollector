using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WOTReplayCollector
{
    enum ReplayWotVersion
    {
        Ver_9_15_1_1 = 49,
        Ver_9_15_2 = 51,
        Ver_9_16 = 50     
    }

    class ReplayCollector
    {
        public string Url { get; set; }

        public string [] TitleKeywords { get; set; }

        public string [] DescKeywords { get; set; }

        public string WOTVersion { get;  set; }

        public ReplayWotVersion Version { get; set; }

        public ReplayCollector(string url, string[] titleKeywords, string[] descKeywords, 
                               ReplayWotVersion version = ReplayWotVersion.Ver_9_16)
        {
            Url = url;
            Version = version;

            TitleKeywords = titleKeywords;
            DescKeywords = descKeywords;
        }

        public ReplayInfo[] Collect(int pages)
        {
            var result = new List<ReplayInfo>();
            var client = new HttpClient();
            bool endOfCollecting = false;
            int pageIndex = 0;            

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

                if (TitleKeywords != null && TitleKeywords.Length > 0)
                {
                    foreach (var replay in replays)
                    {
                        foreach (var keyword in TitleKeywords)
                        {
                            var keywords = new String[]
                            {
                                String.Format("{0} ", keyword.ToUpper()),
                                String.Format(" {0}", keyword.ToUpper()),
                                String.Format("{0} ", keyword.ToUpper()),
                                String.Format(" {0}", keyword.ToUpper())
                            };

                            if (keywords.Any(s => replay.Description.ToUpper().Contains(s)
                                                  || replay.Title.ToUpper().Contains(s)))
                            {
                                result.Add(replay);
                            }
                        }
                    }
                }

                if (DescKeywords != null && DescKeywords.Length > 0)
                {
                    foreach (var replay in replays)
                    {
                        foreach (var keyword in DescKeywords)
                        {
                            var keywords = new String[]
                            {
                                String.Format("{0} ", keyword.ToUpper()),
                                String.Format(" {0}", keyword.ToUpper()),
                                String.Format("{0} ", keyword.ToUpper()),
                                String.Format(" {0}", keyword.ToUpper())
                            };

                            if (keywords.Any(s => replay.Description.ToUpper().Contains(s)
                                                  || replay.Title.ToUpper().Contains(s)))
                            {
                                result.Add(replay);
                            }
                        }
                    }
                }

                if((TitleKeywords == null || TitleKeywords.Length == 0) &&
                   (DescKeywords == null || DescKeywords.Length == 0))
                {
                    result.AddRange(replays);
                }

                --pages;
                ++pageIndex;

                Console.WriteLine("Collecting page #{0}. Number of replays collected: {1}\n", pageIndex + 1, result.Count);

                /* set up next page to be fetched */
                currentUrl = GetNextUrl(pageIndex);
            }          

            client.Dispose();            

            return result.ToArray();
        }

        string GetNextUrl(int index)
        {
            return String.Format("http://wotreplays.com/site/index/version/{0}/sort/uploaded_at.desc/page/{1}/", (int)Version, index);
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
                try
                {
                    var title = node.SelectSingleNode(".//a[@class='link--pale_orange']").InnerText;
                    string url = "http://wotreplays.com" + node.SelectSingleNode(".//a[@class='link--pale_orange']").Attributes["href"].Value;
                    var gameInfo = node.SelectSingleNode("//ul[@class='r-info_ci']");
                    string tank = gameInfo.ChildNodes[1].InnerText.Substring(6);
                    string map = gameInfo.ChildNodes[3].InnerText.Substring(5);
                    string sent = gameInfo.ChildNodes[5].InnerText.Substring(6);
                    string player = gameInfo.ChildNodes[7].InnerText.Substring(8);
                    string description = string.Empty;

                    // Get description from subpage
                    var httpResult = client.GetAsync(url).Result;
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
                catch
                {
                    break;
                }
            }

            client.Dispose();

            return result;
        }
    }
}
