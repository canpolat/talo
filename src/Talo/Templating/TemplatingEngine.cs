using System.Globalization;
using System.Text;

namespace Talo.Templating;

public static class TemplatingEngine
{
    public static RecordContentToWrite GenerateContent(TemplateMap map, string template)
    {
        var content = template;
        foreach (var (key, value) in map.GetKeyValuePairs())
        {
            var searchTermBuilder = new StringBuilder();
            searchTermBuilder.Append("{{");
            searchTermBuilder.Append(key);
            searchTermBuilder.Append("}}");
            var searchTerm = searchTermBuilder.ToString();

            content = content.Replace(searchTerm, value);
        }

        var filename = CreateFileName(map.Type, map.SequenceNumber, map.Title);
        return new RecordContentToWrite(filename, content);
    }

    public static string CreateFileId(string prefix, int sequenceNumber)
    {
        return $"{prefix}{sequenceNumber:0000}";
    }

    public static async Task<string> GetTemplateStringAsync(FileSystemInfo taloRootDir, string templatePath)
    {
        var filePath = Path.IsPathRooted(templatePath)
            ? templatePath
            : Path.Join(taloRootDir.FullName, templatePath);

        if (!File.Exists(filePath))
        {
            throw new ArgumentException($"Could not find file {filePath}");
        }

        return await File.ReadAllTextAsync(filePath, Encoding.UTF8);
    }
    
    private static string CreateFileName(string type, int sequenceNumber, string title)
    {
        var slug = CreateSlugFromTitle(title);
        return $"{type}{sequenceNumber:0000}-{slug}.md";
    }

    private static string CreateSlugFromTitle(string title)
    {
        var titleLowerCase = title.ToLower(CultureInfo.CurrentCulture);
        var sanitizedTitle = ReplaceInvalidCharsFromFilename(titleLowerCase);
        return sanitizedTitle.Replace(" ", "-");
    }

    private static string ReplaceInvalidCharsFromFilename(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }
}
