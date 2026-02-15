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
                    articlaData.idf[token.Key] = Math.Log((double)(articlaData.documents.Count+1) / 
                        (articlaData.articleIndex[token.Key].Count+1)) + 1;
                }
            }
        }
        public List<Article> Search(string text)
        {
            text = text.Trim().ToLower();
            var inputTokens = TextProcessor.GenrateToken(text);
            var searchTokens = new List<string>(); 
            foreach(var token in inputTokens)
            {
                if (articlaData.articleIndex.ContainsKey(token))
                {
                    searchTokens.Add(token);
                }
                else
                {
                    var suggestion = FindClosestWord(token, articlaData.articleIndex.Keys.ToHashSet());
                    if(suggestion != null)
                        searchTokens.Add(suggestion);                    
                }
            }
            
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

        static string? FindClosestWord(string token, HashSet<string> vocabulary)
        {
            int minDistance = int.MaxValue;
            string? bestMatch = null;

            foreach (var word in vocabulary)
            {
                int distance = Levenshtein(token, word);

                if (distance < minDistance && distance <= 2)
                {
                    minDistance = distance;
                    bestMatch = word;
                }
            }

            return bestMatch;
        }
        static int Levenshtein(string a, string b)
        {
            int[,] dp = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            }

            return dp[a.Length, b.Length];
        }
    }
}
