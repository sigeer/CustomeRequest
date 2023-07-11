using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using Utility.Common;
using Utility.Extensions;

namespace ResponseProcessor
{
    public class ActionFactory
    {
        public static string? ReadJsonProperty(string input, string jtoken)
        {
            using (JsonDocument document = JsonDocument.Parse(input))
            {
                // 获取根元素
                JsonElement root = document.RootElement;

                // 获取属性值
                JsonElement? valueElement = root.SelectToken(jtoken);
                return valueElement?.ToString();
            }
        }

        public static IHtmlCollection<IElement> ReadElements(string input, string selector)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(input);
            return document.QuerySelectorAll(selector);
        }

        public static List<string> ReadTextFromElement(IHtmlCollection<IElement> input)
        {
            var result = new List<string>();
            foreach (var node in input)
            {
                result.Add(node.TextContent);
            }
            return result;
        }

        public static List<string> ReadInnertHtmlFromElement(IHtmlCollection<IElement> input)
        {
            var result = new List<string>();
            foreach (var node in input)
            {
                result.Add(node.InnerHtml);
            }
            return result;
        }

        public static List<string?> ReadAttributeFromElement(IHtmlCollection<IElement> input, string attributeName)
        {
            var result = new List<string?>();
            foreach (var node in input)
            {
                result.Add(node.GetAttribute(attributeName));
            }
            return result;
        }

        public static List<string> RegReplace(List<string> input, string s1, string s2)
        {
            return input.Select(x => Regex.Replace(x, s1, s2, RegexOptions.IgnoreCase)).ToList();
        }

        public static async Task<string> Download(List<string> input)
        {
            var storageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "store", DateTime.Now.ToString("yyyyMMddHHmmss"));
            var httpReg = new Regex("^http(s)?:\\/\\/[^\\s]*$");
            var links = input.Where(x => httpReg.IsMatch(x)).ToList();

            var imageDirectory = Path.Combine(storageDirectory, "Blob");
            if (!Directory.Exists(imageDirectory))
                Directory.CreateDirectory(imageDirectory);
            await BulkDownload(imageDirectory, links);

            var textList = input.Except(links).ToList();

            var fileName = Guid.NewGuid().ToString("N");

            var textDirectory = Path.Combine(storageDirectory, "Text");
            if (!Directory.Exists(textDirectory))
                Directory.CreateDirectory(textDirectory);
            var textFilePath = Path.Combine(textDirectory, $"{fileName}.txt");
            textList.ForEach(item =>
            {
                File.AppendAllText(textFilePath, item);
            });

            var tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "archive", DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            var zipFilePath = Path.Combine(tempDirectory, $"{fileName}.zip");
            ZipFile.CreateFromDirectory(storageDirectory, zipFilePath);
            return Path.Combine("archive", DateTime.Now.ToString("yyyyMMddHHmmss"), $"{fileName}.zip");
        }

        public static async Task BulkDownload(string dir, List<string> urls, Action<string>? log = null, CancellationToken cancellationToken = default)
        {
            var snowFlake = Utility.GuidHelper.Snowflake.GetInstance(1);
            var data = urls.Distinct().ToDictionary(x => x, x => snowFlake.NextId().ToString());
            using var httpRequestPool = new WorkPool<HttpClient>();
            await Parallel.ForEachAsync(data, cancellationToken, async (item, ct) =>
            {
                if (string.IsNullOrEmpty(item.Key))
                    return;

                var client = httpRequestPool.GetInstance();
                var uri = new Uri(item.Key);
                var result = await client.GetAsync(uri.ToString(), cancellationToken: ct);
                log?.Invoke($"BulkDownload 请求 {item}");
                var fileName = uri.Segments.Last();
                if (!TryGetExtension(fileName, out var extension))
                {
                    var contentType = result.Content.Headers.ContentType?.MediaType;
                    extension = GetExtensionFromContentType(contentType);
                    if (extension == null)
                        return;

                    fileName = item.Value + extension;
                }
                var path = Path.Combine(dir, fileName);
                var fileBytes = await result.Content.ReadAsByteArrayAsync(cancellationToken: ct);
                if (fileBytes.Length > 0)
                    await File.WriteAllBytesAsync(path, fileBytes, ct);
                else
                    log?.Invoke($"BulkDownload for {item}, file bytes = 0");
                httpRequestPool.Return(client);
            });
        }

        private static bool TryGetExtension(string fileName, out string? extension)
        {
            extension = null;
            if (string.IsNullOrEmpty(fileName))
                return false;

            if (fileName.IndexOf('.') == -1)
                return false;

            var lastIndex = fileName.LastIndexOf('.');
            var tempExtension = fileName.Substring(lastIndex, fileName.Length - lastIndex);
            if (tempExtension.GetMimeType() != String.Empty)
            {
                extension = tempExtension;
                return true;
            }
            return false;
        }

        private static string? GetExtensionFromContentType(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return null;
            foreach (var key in StringExtension.MimeMapping.Keys)
            {
                if (StringExtension.MimeMapping[key].ToLower() == contentType.ToLower())
                {
                    return key;
                }
            }
            return null;
        }
    }
}