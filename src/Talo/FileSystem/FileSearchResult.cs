using Talo.Configuration;

namespace Talo.FileSystem;

public record FileSearchResult(bool Found, FileInfo? FilePath)
{
    public DirectoryInfo GetTaloRootDirectory()
    {
        return Found
            ? FilePath.Directory
            : new DirectoryInfo(Directory.GetCurrentDirectory());
    }

    public TaloConfiguration GetConfiguration()
    {
        return Found
            ? TaloConfiguration.LoadFromFile(FilePath!)
            : TaloConfiguration.Default();
    }
}
