namespace Talo.FileSystem;

public static class FileSystemSearcher
{
    public static FileSearchResult SearchConfigurationFile()
    {
        return SearchFile(Constants.ConfigurationFileName);
    }

    private static FileSearchResult SearchFile(string fileName)
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        if (currentDirectory is null)
        {
            throw new DirectoryNotFoundException("Unable to get the current working directory");
        }

        while (currentDirectory is not null && currentDirectory.FullName != currentDirectory.Root.FullName)
        {
            var filePath = Path.Join(currentDirectory.FullName, fileName);
            if (File.Exists(fileName))
            {
                return new FileSearchResult(Found: true, FilePath: new FileInfo(filePath));
            }

            currentDirectory = currentDirectory.Parent;
        }

        return new FileSearchResult(Found: false, FilePath: null);
    }
}
