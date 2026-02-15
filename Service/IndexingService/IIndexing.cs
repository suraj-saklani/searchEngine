using searchEngineWebApp.Model;

namespace searchEngineWebApp.Service.IndexingService
{
    public interface IIndexing
    {
        void IndexData(string path);
        List<Article> Search(string text);
    }
}