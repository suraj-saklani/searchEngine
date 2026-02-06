using searchEngineWebApp.Model;

namespace searchEngineWebApp.Service
{
    public class Indexing : IIndexing
    {
        private readonly ArticlaData articlaData;
        private readonly object _lockObject = new();
        public Indexing(ArticlaData articlaData)
        {
            this.articlaData = articlaData;
        }

        public void IndexData(string path)
        {
            var fileName = Path.GetFileName(path);
            var text = FileConverter.ConvertPDFToText(path, fileName);
            var tokens = TextProcessor.GenrateToken(text);

            Dictionary<string, int> tokenWithCount = new Dictionary<string, int>();
            foreach (var token in tokens)
            {
                tokenWithCount[token] = tokenWithCount.GetValueOrDefault(token) + 1;
            }

            foreach (var token in tokenWithCount)
            {
                lock (_lockObject)
                {
                    if (!articlaData.articleIndex.ContainsKey(token.Key))
                        articlaData.articleIndex[token.Key] = new List<Model.Article>();

                    articlaData.articleIndex[token.Key].Add(new Model.Article()
                    {
                        Document = fileName,
                        Count = tokenWithCount[token.Key]
                    });
                }
            }
        }
        public List<Article> Search(string text)
        {
            text = text.Trim().ToLower();
            if (articlaData.articleIndex.ContainsKey(text))
            {
                return articlaData.articleIndex[text].OrderByDescending(x => x.Count).ToList();
            }
            return new List<Article>();
        }
    }
}
