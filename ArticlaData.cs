using searchEngineWebApp.Model;
using System.Collections.Concurrent;

namespace searchEngineWebApp
{
    public class ArticlaData
    {
        public ConcurrentDictionary<string, List<Article>> articleIndex =  new ConcurrentDictionary<string, List<Article>>();

    }
}
