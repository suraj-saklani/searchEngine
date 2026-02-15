using searchEngineWebApp.Model;
using UglyToad.PdfPig.AcroForms.Fields;
using UglyToad.PdfPig.Tokens;

namespace searchEngineWebApp.Service.IndexingService
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

            articlaData.documents.Add(new Document { Name = fileName, TotalWords = tokenWithCount.Count });
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
                    articlaData.idf[token.Key] = Math.Log((double)articlaData.documents.Count / 
                        articlaData.articleIndex[token.Key].Count);
                }
            }
        }
        public List<Article> Search(string text)
        {
            text = text.Trim().ToLower();
            var searchTokens = TextProcessor.GenrateToken(text);

            Dictionary<string, double> doc_tf_idf = new Dictionary<string, double>();

            foreach (var token in searchTokens)
            {
                var articls = articlaData.articleIndex[token];
                var idf = articlaData.idf[token];

                foreach (var article in articls)
                {
                    var docId = article.Document;
                    int termF = article.Count;
                    var totalWordInDoc = articlaData.documents.Where(x => x.Name == docId)
                        .Select(x => x.TotalWords).FirstOrDefault();
                    double tf = (double)termF / totalWordInDoc;

                    var tfIdf = tf * idf;
                    doc_tf_idf[docId] = doc_tf_idf.GetValueOrDefault(docId) + tfIdf;
                }

            }
            return doc_tf_idf
            .OrderByDescending(x => x.Value)
            .Select(x=> new Article
            {
                Document = x.Key
            })
            .ToList();
        }
    }
}
