using searchEngineWebApp.Model;

namespace searchEngineWebApp.Service
{
    public interface IIndexing
    {
        void IndexData(string path);
        List<Article> Search(string text);
    }
}