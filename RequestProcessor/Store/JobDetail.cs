using RequestProcessor.Models;
using SQLite;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace RequestProcessor
{
    public class JobDetail
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public string Method { get; set; } = "GET";
        [Required]
        public string Url { get; set; } = null!;
        public string? HttpRequestHeader { get; set; }
        public string? HttpRequestBody { get; set; }

        public string StepJson { get; set; } = "[]";

        public HttpRequestHeaderItem[] GetHeaderObj()
        {
            return JsonSerializer.Deserialize<HttpRequestHeaderItem[]>(HttpRequestHeader ?? "[]", new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new HttpRequestHeaderItem[] { };
        }

        public List<StepNode> GetSteps()
        {
            return JsonSerializer.Deserialize<List<StepNode>>(StepJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<StepNode>();
        }
    }
}