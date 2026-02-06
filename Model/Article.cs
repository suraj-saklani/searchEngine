namespace searchEngineWebApp.Model
{
    public class Article
    {
        public string Document { get; set; }
        public int Count { get; set; }
    }
    public record IndexRequest(string Path);
}
