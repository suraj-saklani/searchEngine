using searchEngineWebApp.Model;
using System.Collections.Concurrent;

namespace searchEngineWebApp
{
    public class ArticlaData
    {
        public ConcurrentDictionary<string, List<Article>> articleIndex =  new ConcurrentDictionary<string, List<Article>>();

        public ConcurrentBag<Document> documents = new ();

        public ConcurrentDictionary<string, double> idf = new ConcurrentDictionary<string, double>();


    }
}
