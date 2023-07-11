namespace RequestProcessor.Models
{
    public class HttpRequestHeaderItem
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;

        public bool IsContentType()
        {
            return new string[] { "content-type", "contenttype" }.Contains(Key.ToLower());
        }
    }
}
