namespace Talo.Configuration;

public interface IRecordConfiguration
{
    string Name { get; }
    string Description { get; }
    string Location { get; }
    string Prefix { get; }
    string TemplatePath { get; }
    IRecordConfiguration GetInitializedConfiguration();
    string GetRecordDirectoryPath(FileSystemInfo taloRootDir);
    IRecordConfiguration With(string location, string templatePath);
    Task<string> GetTemplateStringAsync(FileSystemInfo taloRootDir);

    bool IsInitialized()
    {
        return !string.IsNullOrWhiteSpace(Location);
    }
}
