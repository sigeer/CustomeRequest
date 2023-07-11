using System.ComponentModel.DataAnnotations;

namespace RequestProcessor.Models
{
    public class HttpRequestUnit
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public string Method { get; set; } = "GET";
        [Required]
        public string Url { get; set; } = null!;
        public HttpRequestHeaderItem[] HttpRequestHeader { get; set; } = new HttpRequestHeaderItem[] { };
        public string? HttpRequestBody { get; set; }

        public List<StepNode> Steps { get; set; } = new List<StepNode>();
    }
}
