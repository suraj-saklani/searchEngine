using System.Text.RegularExpressions;

namespace searchEngineWebApp.Service
{
    public class TextProcessor
    {
        static List<string> stopWords = new()
        {
              "the","is","are","and","or","to","of","in","on","for","with",
            "as","by","an","a","this","that","it","be","from","at"
        };
        
        public static List<string> GenrateToken(string content)
        {
            var lower = content.ToLower();

            var words = Regex.Split(lower, @"\W+");

            return words.Where(x=>x.Length > 2 && !stopWords.Contains(x)).ToList();
        } 
    }
}
