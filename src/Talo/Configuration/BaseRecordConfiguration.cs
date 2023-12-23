using System.Globalization;
using Talo.Templating;

namespace Talo.Configuration;

public class BaseRecordConfiguration
    : IRecordConfiguration
{
    public virtual string Name { get; init; }

    public string Description { get; init; }

    public string Location { get; init; }
    public string Prefix { get; init; }
    public string TemplatePath { get; init; }

    public IRecordConfiguration GetInitializedConfiguration()
    {
        var location = string.IsNullOrWhiteSpace(Location)
            ? Name
            : Location;

        var prefix = string.IsNullOrWhiteSpace(Prefix)
            ? Name.ToUpper(CultureInfo.CurrentCulture)
            : Prefix;

        return new BaseRecordConfiguration
        {
            Name = Name,
            Description = Description,
            Location = location,
            Prefix = prefix,
            TemplatePath = TemplatePath
        };
    }

    public string GetRecordDirectoryPath(FileSystemInfo taloRootDir)
    {
        return Path.Join(taloRootDir.FullName, Location);
    }
    
    public async Task<string> GetTemplateStringAsync(FileSystemInfo taloRootDir)
    {
        if (string.IsNullOrWhiteSpace(TemplatePath))
        {
            throw new InvalidOperationException(
                $"Template path is not set for this record type ({Name})");
        }

        return await TemplatingEngine.GetTemplateStringAsync(taloRootDir, TemplatePath);
    }

    public IRecordConfiguration With(string location, string templatePath)
    {
        location = string.IsNullOrWhiteSpace(location)
            ? Location
            : location;

        templatePath = string.IsNullOrWhiteSpace(templatePath)
            ? TemplatePath
            : templatePath;

        return new BaseRecordConfiguration()
        {
            Name = Name,
            Description = Description,
            Location = location,
            Prefix = Prefix,
            TemplatePath = templatePath
        };
    }
}
